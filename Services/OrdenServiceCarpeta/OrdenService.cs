using Mini_E_Commerce_API.DALs;
using Mini_E_Commerce_API.DALs.CarritoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.OrdenRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.OrdenDtoCarpeta;
using Mini_E_Commerce_API.Models;
using System.Linq.Expressions;

namespace Mini_E_Commerce_API.Services.OrdenServiceCarpeta
{
    public class OrdenService : IOrdenService
    {
        private readonly IOrdenRepository _ordenRepository;
        private readonly ICarritoRepository _carritoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IProductoRepository _productoRepository;

        public OrdenService(IOrdenRepository ordenRepository, IUsuarioRepository usuarioRepository, IUnidadDeTrabajo unidadDeTrabajo,ICarritoRepository carritoRepository,IProductoRepository productoRepository) { 
            _ordenRepository = ordenRepository;
            _usuarioRepository = usuarioRepository;
            _unidadDeTrabajo = unidadDeTrabajo; 
            _carritoRepository = carritoRepository;
            _productoRepository = productoRepository;
        }


        public async Task<Result<OrdenDto>> CrearOrdenAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuario == null) {
                return Result<OrdenDto>.Failure($"Su usuario con id {usuarioId} no existe");
            }

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

                if(item.Quatity > item.Producto.Stock)
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
    }
}
