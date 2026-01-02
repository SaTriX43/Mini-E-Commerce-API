using Mini_E_Commerce_API.DALs.CarritoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.CarritoServiceCarpeta
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IProductoRepository _productoRepository;

        public CarritoService(ICarritoRepository carritoRepository, IUsuarioRepository usuarioRepository, IProductoRepository productoRepository) { 
            _carritoRepository = carritoRepository;
            _usuarioRepository = usuarioRepository;
            _productoRepository = productoRepository;
        }

        public async Task<Result<CarritoDto>> ObtenerCarritoPorUsuarioIdAsync(int usuarioId)
        {
            var usuarioExiste = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if(usuarioExiste == null)
            {
                return Result<CarritoDto>.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            var carrito = await _carritoRepository.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if(carrito == null)
            {
                var carritoModel = new Carrito
                {
                    UserId = usuarioId,
                    CreatedAt = DateTime.UtcNow,
                };
                carrito = _carritoRepository.CrearCarrito(carritoModel);
                await _carritoRepository.GuardarCambiosAsync();
            }

            decimal total = 0;

            foreach (var carritoItem in carrito.Items)
            {
                decimal subtotal = carritoItem.Quatity * carritoItem.Producto.Price;
                total += subtotal;
            }

            var carritoDto = new CarritoDto
            {
                Id = carrito.Id,
                CreatedAt = carrito.CreatedAt,
                UpdatedAt = carrito.UpdatedAt,
                UserId = usuarioId,
                Items = carrito.Items.Select(ci => new CarritoItemDto
                {
                    CartId = ci.CartId,
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    Quatity = ci.Quatity,
                    UnitPrice = ci.Producto.Price,
                    SubTotal = ci.Quatity * ci.Producto.Price,
                }).ToList(),
                Total = total
            };

            return Result<CarritoDto>.Success(carritoDto);
        }

        public async Task<Result> AgregarCarritoItemAsync(CarritoItemAgregarDto itemAgregarDto, int usuarioId)
        {
            if(itemAgregarDto.Quantity <= 0)
            {
                return Result.Failure("La cantidad no debe de ser menor o igual a 0");
            }

            if (itemAgregarDto.ProductId <= 0)
            {
                return Result.Failure("El productoId no debe de ser menor o igual a 0");
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if(usuario == null)
            {
                return Result.Failure($"Su usuario con id {usuarioId} no existe");
            }

            var carrito = await _carritoRepository.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if (carrito == null) {
                var carritoModel = new Carrito
                {
                    UserId = usuarioId,
                    CreatedAt = DateTime.UtcNow,
                };
                carrito = _carritoRepository.CrearCarrito(carritoModel);
                await _carritoRepository.GuardarCambiosAsync();
            }

            var producto = await _productoRepository.ObtenerProductoPorIdAsync(itemAgregarDto.ProductId);

            if(producto == null)
            {
                return Result.Failure($"Su producto con id {itemAgregarDto.ProductId} no existe");
            }

            if(!producto.IsActive)
            {
                return Result.Failure($"Su producto con id {itemAgregarDto.ProductId} esta inactivo");
            }
            
            if(itemAgregarDto.Quantity > producto.Stock)
            {
                return Result.Failure($"Su producto con id {itemAgregarDto.ProductId} no tiene suficiente stock");
            }

            var carritoItem = await _carritoRepository.ObtenerCarritoItemAsync(carrito.Id,producto.Id);

            if (carritoItem != null)
            {
                if(carritoItem.Quatity + itemAgregarDto.Quantity > producto.Stock)
                {
                    return Result.Failure($"Su producto con id {itemAgregarDto.ProductId} no tiene suficiente stock");
                }
                carritoItem.Quatity += itemAgregarDto.Quantity;
            }else
            {
                var carritoItemModel = new CarritoItem
                {
                    CartId = carrito.Id,
                    ProductId = producto.Id,
                    Quatity = itemAgregarDto.Quantity, 
                };

                _carritoRepository.AgregarCarritoItem(carritoItemModel); 
            }

            carrito.UpdatedAt = DateTime.UtcNow;

            await _carritoRepository.GuardarCambiosAsync();

            return Result.Success();
        }
    }
}
