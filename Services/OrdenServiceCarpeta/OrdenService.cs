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

        public OrdenService(IOrdenRepository ordenRepository, IUnidadDeTrabajo unidadDeTrabajo, ICarritoRepository carritoRepository, IUsuarioRepository usuarioRepository) {
            _ordenRepository = ordenRepository;
            _unidadDeTrabajo = unidadDeTrabajo;
            _carritoRepository = carritoRepository;
            _usuarioRepository = usuarioRepository;
        }


        //USUARIO

        public async Task<Result<OrdenDto>> CrearOrdenAsync(int usuarioId)
        {
            var carrito = await _carritoRepository.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if (carrito == null) {
                return Result<OrdenDto>.Failure("Carrito no existe");
            }

            if (carrito.Items.Count == 0) {
                return Result<OrdenDto>.Failure("No se pudo crear orden si no hay items en carrito");
            }

            decimal montoTotal = 0;

            foreach (var item in carrito.Items) {

                if (item.Producto == null || !item.Producto.IsActive) {
                    return Result<OrdenDto>.Failure($"No se pudo crear orden por, Producto con id {item.ProductId} nullo o inactivo");
                }

                if (item.Quatity > item.Producto.Stock)
                {
                    return Result<OrdenDto>.Failure($"No se pudo crear orden por, Producto con id {item.ProductId} no tiene suficiente stock");
                }

                montoTotal += item.Quatity * item.Producto.Price;
            }

            var ordenModel = new Orden
            {
                Status = Models.Enums.StatusOrden.Pending,
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
                return Result<OrdenDto>.Failure("Su orden id no debe de ser menor o igual a 0");
            }

            var orden = await _ordenRepository.ObtenerOrdenPorOrdenIdAsync(ordenId);

            if (orden == null || orden.UserId != usuarioId )
            {
                return Result<OrdenDto>.Failure($"Orden no existe");
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
                return Result.Failure("Su orden id no debe de ser menor o igual a 0");
            }

            var orden = await _ordenRepository.ObtenerOrdenPorOrdenIdAsync(ordenId);

            if (orden == null || orden.UserId != usuarioId)
            {
                return Result.Failure($"Orden no existe");
            }

            if (orden.Status == StatusOrden.Cancelled)
            {
                return Result.Failure("La orden ya está cancelada");
            }

            if (orden.Status != StatusOrden.Pending)
            {
                return Result.Failure("La orden no se puede cancelar");
            }


            orden.Status = StatusOrden.Cancelled;
            await _unidadDeTrabajo.GuardarCambiosAsync();

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
            if(usuarioId <= 0)
            {
                return Result<List<OrdenDto>>.Failure("El usuarioId no debe ser menor o igual a 0");
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if(usuario == null)
            {
                return Result<List<OrdenDto>>.Failure($"Usuario con id = {usuarioId} no existe");
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
    }
}
