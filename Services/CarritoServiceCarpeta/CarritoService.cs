using Mini_E_Commerce_API.DALs.CarritoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.CarritoServiceCarpeta
{
    public class CarritoService : ICarritoService
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CarritoService(ICarritoRepository carritoRepository, IUsuarioRepository usuarioRepository) { 
            _carritoRepository = carritoRepository;
            _usuarioRepository = usuarioRepository;
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
                carrito = await _carritoRepository.CrearCarritoAsync(carritoModel);
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
    }
}
