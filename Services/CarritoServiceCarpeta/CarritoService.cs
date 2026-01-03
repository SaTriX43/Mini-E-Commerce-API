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

            var ctx = await ObtenerContextoCarritoAsync(usuarioId, itemAgregarDto.ProductId);

            if(!ctx.IsSuccess)
            {
                return Result.Failure(ctx.Error);
            }

            var item = ctx.Value.Item;
            var producto = ctx.Value.Producto;
            var carrito = ctx.Value.Carrito;

            if(item == null && itemAgregarDto.Quantity > producto.Stock)
            {
                return Result.Failure($"Su producto con id {itemAgregarDto.ProductId} no tiene suficiente stock");
            }

            if (item != null)
            {
                if(item.Quatity + itemAgregarDto.Quantity > producto.Stock)
                {
                    return Result.Failure($"Su producto con id {itemAgregarDto.ProductId} no tiene suficiente stock");
                }
                item.Quatity += itemAgregarDto.Quantity;
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
        public async Task<Result> ActualizarCantidadCarritoItemAsync(CarritoItemAgregarDto itemAgregarDto, int usuarioId)
        {
            if (itemAgregarDto.Quantity <= 0)
            {
                return Result.Failure("La cantidad no debe de ser menor o igual a 0");
            }

            var ctx = await ObtenerContextoCarritoAsync(usuarioId, itemAgregarDto.ProductId);

            if (!ctx.IsSuccess)
            {
                return Result.Failure(ctx.Error);
            }

            var item = ctx.Value.Item;
            var producto = ctx.Value.Producto;
            var carrito = ctx.Value.Carrito;

            if (item == null)
            {
                return Result.Failure($"Su item no existe en el carrito");
            }
            
            if (itemAgregarDto.Quantity > producto.Stock)
            {
                return Result.Failure($"Su producto con id {itemAgregarDto.ProductId} no tiene suficiente stock");
            }

            item.Quatity = itemAgregarDto.Quantity;

            carrito.UpdatedAt = DateTime.UtcNow;

            await _carritoRepository.GuardarCambiosAsync();

            return Result.Success();
        }
        public async Task<Result> EliminarCarritoItemAsync(int carritoItemId, int usuarioId)
        {
            if(carritoItemId <= 0)
            {
                return Result.Failure("El id de carritoItem no puede ser menor o igual a 0");
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if(usuario == null)
            {
                return Result.Failure($"Usuario con id = {usuarioId} no existe");
            }
            
            var carritoItem = await _carritoRepository.ObtenerCarritoItemPorIdAsync(carritoItemId);

            if (carritoItem == null)
            {
                return Result.Failure("El item no existe");
            }

            if (carritoItem.Carrito.UserId != usuarioId) {
                return Result.Failure("No tiene permiso para eliminar este item");
            }


            _carritoRepository.EliminarItemCarrito(carritoItem);
            carritoItem.Carrito.UpdatedAt = DateTime.UtcNow;
            await _carritoRepository.GuardarCambiosAsync();

            return Result.Success();
        }
        public async Task<Result> VaciarCarritoAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if(usuario == null)
            {
                return Result.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            var carrito = await _carritoRepository.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if(carrito == null)
            {
                return Result.Failure("Carrito no existe");
            }

            await _carritoRepository.VaciarCarritoItems(carrito.Id);
            carrito.UpdatedAt = DateTime.UtcNow;
            await _carritoRepository.GuardarCambiosAsync();

            return Result.Success();
        }


        private async Task<Result<ContextoCarritoDto>> ObtenerContextoCarritoAsync(int usuarioId, int productoId)
        {
            if (productoId <= 0)
            {
                return Result<ContextoCarritoDto>.Failure("El productoId no debe de ser menor o igual a 0");
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
            {
                return Result<ContextoCarritoDto>.Failure($"Su usuario con id {usuarioId} no existe");
            }

            var carrito = await _carritoRepository.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if (carrito == null)
            {
                var carritoModel = new Carrito
                {
                    UserId = usuarioId,
                    CreatedAt = DateTime.UtcNow,
                };
                carrito = _carritoRepository.CrearCarrito(carritoModel);
            }

            var producto = await _productoRepository.ObtenerProductoPorIdAsync(productoId);

            if (producto == null)
            {
                return Result<ContextoCarritoDto>.Failure($"Su producto con id {productoId} no existe");
            }

            if (!producto.IsActive)
            {
                return Result<ContextoCarritoDto>.Failure($"Su producto con id {productoId} esta inactivo");
            }

            var carritoItem = await _carritoRepository.ObtenerCarritoItemAsync(carrito.Id, producto.Id);

            return Result<ContextoCarritoDto>.Success(new ContextoCarritoDto
            {
                Producto = producto,
                Carrito = carrito,
                Item = carritoItem,
                Usuario = usuario,
            });
        }
    }
}
