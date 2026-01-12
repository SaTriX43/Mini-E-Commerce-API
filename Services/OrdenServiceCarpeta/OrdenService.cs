using Mini_E_Commerce_API.Common.Errors;
using Mini_E_Commerce_API.Common.Results;
using Mini_E_Commerce_API.DALs;
using Mini_E_Commerce_API.DALs.CarritoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.OrdenRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.OrdenDtoCarpeta;
using Mini_E_Commerce_API.Models;
using Mini_E_Commerce_API.Models.Enums;
using System.Linq.Expressions;

namespace Mini_E_Commerce_API.Services.OrdenServiceCarpeta
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly ICarritoRepository _carritoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly ILogger<OrdenService> _logger;

        public OrdenService(
            IOrdenRepository ordenRepository,
            IUnidadDeTrabajo unidadDeTrabajo,
            ICarritoRepository carritoRepository,
            IUsuarioRepository usuarioRepository,
            ILogger<OrdenService> logger)
        {
            _ordenRepository = ordenRepository;
            _unidadDeTrabajo = unidadDeTrabajo;
            _carritoRepository = carritoRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        //USUARIO

        public async Task<Result<OrdenDto>> CrearOrdenAsync(int usuarioId)
        {
            var carrito = await _carritoRepository.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if (carrito == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "CrearOrden", "CARRITO_NO_EXISTE", usuarioId);
                return Result<OrdenDto>.Failure(DomainErrors.Cart.NotFound);
            }

            if (carrito.Items.Count == 0)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} CartId={CartId}", "CrearOrden", "CARRITO_VACIO", usuarioId, carrito.Id);
                return Result<OrdenDto>.Failure(DomainErrors.Order.EmptyCart);
            }

            decimal montoTotal = 0;

            foreach (var item in carrito.Items)
            {
                if (item.Producto == null || !item.Producto.IsActive)
                {
                    _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} CartId={CartId} ProductId={ProductId}", "CrearOrden", "PRODUCTO_INACTIVO_O_NULO", usuarioId, carrito.Id, item.ProductId);

                    return Result<OrdenDto>.Failure(DomainErrors.Product.NotFound(item.ProductId));
                }

                if (item.Quatity > item.Producto.Stock)
                {
                    _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} CartId={CartId} ProductId={ProductId}", "CrearOrden", "STOCK_INSUFICIENTE", usuarioId, carrito.Id, item.ProductId);
                    return Result<OrdenDto>.Failure(DomainErrors.Product.InssuficientStock(item.ProductId));
                }

                montoTotal += item.Quatity * item.Producto.Price;
            }

            var ordenModel = new Orden
            {
                Status = StatusOrden.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = montoTotal,
                UserId = usuarioId,
                Detalles = carrito.Items.Select(i => new OrdenDetalle
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quatity,
                    Subtotal = i.Quatity * i.Producto.Price,
                    UnitPrice = i.Producto.Price,
                }).ToList()
            };

            var ordenCreada = _ordenRepository.CrearOrden(ordenModel);

            await _unidadDeTrabajo.GuardarCambiosAsync();

            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId} CartId={CartId}", "CrearOrden", "OK", usuarioId, ordenCreada.Id, carrito.Id);

            var ordenDto = new OrdenDto
            {
                Status = ordenCreada.Status,
                CreatedAt = ordenCreada.CreatedAt,
                TotalAmount = ordenCreada.TotalAmount,
                UserId = ordenCreada.UserId,
                Id = ordenCreada.Id,
                OrdenDetallesDtos = ordenCreada.Detalles.Select(i => new OrdenDetallesDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Subtotal = i.Subtotal,
                    UnitPrice = i.Producto.Price,
                    Id = i.Id,
                    OrderId = i.OrderId,
                }).ToList()
            };

            return Result<OrdenDto>.Success(ordenDto);
        }

        public async Task<Result<List<OrdenDto>>> ObtenerOrdenesUsuarioAsync(int usuarioId)
        {
            var ordenes = await _ordenRepository.ObtenerOrdenesPorUsuarioIdAsync(usuarioId);

            var ordenesDto = ordenes.Select(o => new OrdenDto
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                OrdenDetallesDtos = o.Detalles.Select(od => new OrdenDetallesDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Subtotal = od.Subtotal,
                    UnitPrice = od.UnitPrice
                }).ToList(),
            }).ToList();

            return Result<List<OrdenDto>>.Success(ordenesDto);
        }

        public async Task<Result<OrdenDto>> ObtenerOrdenDetallesUsuarioAsync(int ordenId, int usuarioId)
        {
            if (ordenId <= 0)
            {
                return Result<OrdenDto>.Failure(DomainErrors.Order.InvalidId);
            }

            var orden = await _ordenRepository.ObtenerOrdenPorOrdenIdAsync(ordenId);

            if (orden == null || orden.UserId != usuarioId)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "ObtenerOrdenDetalle", "ORDEN_NO_EXISTE_O_NO_PERTENECE", usuarioId, ordenId);
                return Result<OrdenDto>.Failure(DomainErrors.Order.NotFound);
            }

            var ordenDto = new OrdenDto
            {
                UserId = orden.UserId,
                CreatedAt = orden.CreatedAt,
                Id = ordenId,
                Status = orden.Status,
                TotalAmount = orden.TotalAmount,
                OrdenDetallesDtos = orden.Detalles.Select(od => new OrdenDetallesDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Subtotal = od.Subtotal,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

            return Result<OrdenDto>.Success(ordenDto);
        }

        public async Task<Result> CancelarOrdenAsync(int ordenId, int usuarioId)
        {
            if (ordenId <= 0)
            {
                return Result.Failure(DomainErrors.Order.InvalidId);
            }

            var orden = await _ordenRepository.ObtenerOrdenPorOrdenIdAsync(ordenId);

            if (orden == null || orden.UserId != usuarioId)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "CancelarOrden", "ORDEN_NO_EXISTE_O_NO_PERTENECE", usuarioId, ordenId);
                return Result.Failure(DomainErrors.Order.NotFound);
            }

            if (orden.Status == StatusOrden.Cancelled)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "CancelarOrden", "YA_CANCELADA", usuarioId, ordenId);
                return Result.Failure(DomainErrors.Order.AlreadyCancelled);
            }

            if (orden.Status != StatusOrden.Pending)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "CancelarOrden", "ESTADO_INVALIDO", usuarioId, ordenId);
                return Result.Failure(DomainErrors.Order.InvalidStatusToCancel);
            }

            orden.Status = StatusOrden.Cancelled;
            await _unidadDeTrabajo.GuardarCambiosAsync();

            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "CancelarOrden", "OK", usuarioId, ordenId);

            return Result.Success();
        }

        public async Task<Result> PagarOrdenAsync(int ordenId, int usuarioId)
        {
            if (ordenId <= 0)
            {
                return Result.Failure(DomainErrors.Order.InvalidId);
            }

            var orden = await _ordenRepository.ObtenerOrdenParaPagoAsync(ordenId);

            if (orden == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "PagarOrden", "ORDEN_NO_EXISTE", usuarioId, ordenId);
                return Result.Failure(DomainErrors.Order.NotFound);
            }

            if (orden.UserId != usuarioId)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "PagarOrden", "ORDEN_NO_PERTENECE", usuarioId, ordenId);
                return Result.Failure(DomainErrors.Order.NotFound);
            }

            if (orden.Status != StatusOrden.Pending)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId}", "PagarOrden", "ESTADO_INVALIDO", usuarioId, ordenId);
                return Result.Failure(DomainErrors.Order.InvalidStatusAction);
            }

            foreach (var detalle in orden.Detalles)
            {
                if (detalle.Producto.Stock < detalle.Quantity)
                {
                    _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId} ProductId={ProductId}", "PagarOrden", "STOCK_INSUFICIENTE", usuarioId, ordenId, detalle.ProductId);
                    return Result.Failure(DomainErrors.Product.InssuficientStock(detalle.ProductId));
                }
            }

            foreach (var detalle in orden.Detalles)
            {
                detalle.Producto.Stock -= detalle.Quantity;
            }

            var carrito = await _carritoRepository.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if (carrito != null)
            {
                await _carritoRepository.VaciarCarritoItems(carrito.Id);
                carrito.UpdatedAt = DateTime.UtcNow;
            }

            orden.Status = StatusOrden.Paid;

            await _unidadDeTrabajo.GuardarCambiosAsync();

            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId} OrderId={OrderId} CartId={CartId}", "PagarOrden", "OK", usuarioId, ordenId, carrito?.Id);

            return Result.Success();
        }

        //ADMIN
        public async Task<Result<List<OrdenDto>>> ObtenerTodasLasOrdenesAsync()
        {
            var ordenes = await _ordenRepository.ObtenerTodasLasOrdenesAsync();

            var ordenesDto = ordenes.Select(o => new OrdenDto
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                OrdenDetallesDtos = o.Detalles.Select(od => new OrdenDetallesDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Subtotal = od.Subtotal,
                    UnitPrice = od.UnitPrice
                }).ToList(),
            }).ToList();

            return Result<List<OrdenDto>>.Success(ordenesDto);
        }

        public async Task<Result<List<OrdenDto>>> ObtenerOrdenesPorUsuarioAdminAsync(int usuarioId)
        {
            if (usuarioId <= 0)
            {
                return Result<List<OrdenDto>>.Failure(DomainErrors.User.InvalidId);
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "ObtenerOrdenesPorUsuarioAdmin", "USUARIO_NO_EXISTE", usuarioId);
                return Result<List<OrdenDto>>.Failure(DomainErrors.User.NotFound(usuarioId));
            }

            var ordenes = await _ordenRepository.ObtenerOrdenesPorUsuarioIdAsync(usuarioId);

            var ordenesDto = ordenes.Select(o => new OrdenDto
            {
                Id = o.Id,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                OrdenDetallesDtos = o.Detalles.Select(od => new OrdenDetallesDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Subtotal = od.Subtotal,
                    UnitPrice = od.UnitPrice
                }).ToList(),
            }).ToList();

            return Result<List<OrdenDto>>.Success(ordenesDto);
        }

        public async Task<Result<OrdenDto>> ObtenerOrdenDetallesUsuarioAdminAsync(int ordenId)
        {
            if (ordenId <= 0)
            {
                return Result<OrdenDto>.Failure(DomainErrors.Order.InvalidId);
            }

            var orden = await _ordenRepository.ObtenerOrdenPorOrdenIdAsync(ordenId);

            if (orden == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} OrderId={OrderId}", "ObtenerOrdenDetalleAdmin", "ORDEN_NO_EXISTE", ordenId);
                return Result<OrdenDto>.Failure(DomainErrors.Order.NotFound);
            }

            var ordenDto = new OrdenDto
            {
                UserId = orden.UserId,
                CreatedAt = orden.CreatedAt,
                Id = ordenId,
                Status = orden.Status,
                TotalAmount = orden.TotalAmount,
                OrdenDetallesDtos = orden.Detalles.Select(od => new OrdenDetallesDto
                {
                    Id = od.Id,
                    OrderId = od.OrderId,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Subtotal = od.Subtotal,
                    UnitPrice = od.UnitPrice
                }).ToList()
            };

            return Result<OrdenDto>.Success(ordenDto);
        }
    }
}
