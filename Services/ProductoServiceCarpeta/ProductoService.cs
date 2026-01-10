using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mini_E_Commerce_API.DALs;
using Mini_E_Commerce_API.DALs.CategoriaRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta;
using Mini_E_Commerce_API.Models;
using Mini_E_Commerce_API.Models.Enums;

namespace Mini_E_Commerce_API.Services.ProductoServiceCarpeta
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ProductoService> _logger;
        private readonly IConfiguration _configuration;
        private const string ProductosListCacheKey = "products:list";

        public ProductoService(IProductoRepository productoRepository, IUsuarioRepository usuarioRepository, ICategoriaRepository categoriaRepository, IUnidadDeTrabajo unidadDeTrabajo, IMemoryCache memoryCache, ILogger<ProductoService> logger, IConfiguration configuration)
        {
            _productoRepository = productoRepository;
            _usuarioRepository = usuarioRepository;
            _categoriaRepository = categoriaRepository;
            _unidadDeTrabajo = unidadDeTrabajo;
            _memoryCache = memoryCache;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<Result<ProductoDto>> CrearProductoAsync(ProductoCrearDto productoCrearDto, int usuarioId, string rol)
        {
            var usuarioExiste = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuarioExiste == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "CrearProducto", "USUARIO_NO_EXISTE", usuarioId);
                return Result<ProductoDto>.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            if (rol != RolUsuario.Admin.ToString())
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "CrearProducto", "SIN_PERMISOS", usuarioId);
                return Result<ProductoDto>.Failure($"No puede realizar esta accion ya que no es un Administrador");
            }

            if (productoCrearDto.Stock <= 0)
            {
                return Result<ProductoDto>.Failure($"No puede realizar esta accion ya que el stock debe de ser mayor a 0");
            }

            var categoriaExiste = await _categoriaRepository.ObtenerCategoriaPorIdAsync(productoCrearDto.CategoryId);

            if (categoriaExiste == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "CrearProducto", "CATEGORIA_NO_EXISTE", usuarioId);
                return Result<ProductoDto>.Failure($"Su categoria con id = {productoCrearDto.CategoryId} no existe");
            }

            if (!categoriaExiste.IsActive)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "CrearProducto", "CATEGORIA_INACTIVA", usuarioId);
                return Result<ProductoDto>.Failure($"Su categoria con id = {productoCrearDto.CategoryId} esta inactiva");
            }


            if (productoCrearDto.Price <= 0)
            {
                return Result<ProductoDto>.Failure($"El precio no puede ser menor o igual a 0");
            }

            var productoNombreNormzalizado = productoCrearDto.Name.Trim().ToLower();

            var existe = await _productoRepository.ExisteProductoConNombreEnCategoriaAsync(productoNombreNormzalizado, productoCrearDto.CategoryId, null);

            if (existe)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "CrearProducto", "NOMBRE_DUPLICADO", usuarioId);
                return Result<ProductoDto>.Failure(
                    "No puede existir dos productos con el mismo nombre en la misma categoría"
                );
            }

            var productoModel = new Producto
            {
                CategoryId = productoCrearDto.CategoryId,
                Description = productoCrearDto.Description,
                Name = productoNombreNormzalizado,
                Price = productoCrearDto.Price,
                Stock = productoCrearDto.Stock,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            var productoCreado = _productoRepository.CrearProducto(productoModel);

            var productoDto = new ProductoDto
            {
                Id = productoCreado.Id,
                CategoryId = productoCreado.CategoryId,
                Description = productoCreado.Description,
                Name = productoCreado.Name,
                Price = productoCreado.Price,
                Stock = productoCreado.Stock,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                UpdatedAt = productoCreado.UpdatedAt
            };

            await _unidadDeTrabajo.GuardarCambiosAsync();

            _memoryCache.Remove(ProductosListCacheKey);
            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "CrearProducto", "OK", usuarioId, productoCreado.Id);

            return Result<ProductoDto>.Success(productoDto);
        }

        public async Task<Result<ProductoDto>> ObtenerProductoPorIdAsync(int usuarioId, int productoId)
        {
            if (productoId <= 0)
            {
                return Result<ProductoDto>.Failure($"Su productoId no puede ser menor o igual a 0");
            }

            var usuarioExise = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuarioExise == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ObtenerProducto", "USUARIO_NO_EXISTE", usuarioId, productoId);
                return Result<ProductoDto>.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            var productoExiste = await _productoRepository.ObtenerProductoPorIdAsync(productoId);

            if (productoExiste == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ObtenerProducto", "PRODUCTO_NO_EXISTE", usuarioId, productoId);
                return Result<ProductoDto>.Failure($"Su producto con id = {productoId} no existe");
            }

            var productoDto = new ProductoDto
            {
                Id = productoExiste.Id,
                CategoryId = productoExiste.CategoryId,
                Description = productoExiste.Description,
                Name = productoExiste.Name,
                IsActive = productoExiste.IsActive,
                CreatedAt = productoExiste.CreatedAt,
                Price = productoExiste.Price,
                Stock = productoExiste.Stock,
                UpdatedAt = productoExiste.UpdatedAt,
            };

            return Result<ProductoDto>.Success(productoDto);
        }
        public async Task<Result<List<ProductoDto>>> ObtenerProductosAsync(int usuarioId)
        {
            var usuarioExise = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuarioExise == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId}", "ObtenerProductos", "USUARIO_NO_EXISTE", usuarioId);
                return Result<List<ProductoDto>>.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            if (_memoryCache.TryGetValue(ProductosListCacheKey, out List<ProductoDto>? cached))
            {
                _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId}", "ObtenerProductos", "CACHE", usuarioId);
                return Result<List<ProductoDto>>.Success(cached!);
            }

            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId}", "ObtenerProductos", "BD", usuarioId);

            var productos = await _productoRepository.ObtenerProductosAsync();

            var productosDtos = productos.Select(p => new ProductoDto
            {
                Id = p.Id,
                CategoryId = p.CategoryId,
                CreatedAt = p.CreatedAt,
                Price = p.Price,
                Description = p.Description,
                Name = p.Name,
                IsActive = p.IsActive,
                Stock = p.Stock,
                UpdatedAt = p.UpdatedAt,
            }).ToList();

            int duracionCache = _configuration.GetValue<int>(
                "ConfigCache:DuracionCacheSegundos"
            );

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(duracionCache)
            };

            _memoryCache.Set(ProductosListCacheKey, productosDtos, options);

            return Result<List<ProductoDto>>.Success(productosDtos);
        }

        public async Task<Result> OperacionStockProductoAsync(int usuarioId, int productoId, TipoDeMovimiento tipoDeMovimiento, int cantidad)
        {
            var usuarioEncontrado = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuarioEncontrado == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "OperacionStock", "USUARIO_NO_EXISTE", usuarioId, productoId);
                return Result.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            var productoEncontrado = await _productoRepository.ObtenerProductoPorIdAsync(productoId);

            if (productoEncontrado == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "OperacionStock", "PRODUCTO_NO_EXISTE", usuarioId, productoId);
                return Result.Failure($"Su producto con id = {productoId} no existe");
            }

            if (cantidad <= 0)
            {
                return Result.Failure("La cantidad debe de ser mayor a 0");
            }

            if (!productoEncontrado.IsActive)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "OperacionStock", "PRODUCTO_INACTIVO", usuarioId, productoId);
                return Result.Failure($"Su producto con id = {productoId} esta incativo");
            }

            if (tipoDeMovimiento == TipoDeMovimiento.Incrementar)
            {
                productoEncontrado.Stock += cantidad;
            }
            else
            {
                if (productoEncontrado.Stock < cantidad)
                {
                    _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "OperacionStock", "STOCK_INSUFICIENTE", usuarioId, productoId);
                    return Result.Failure($"Su producto con id = {productoId} no tiene stock suficiente");
                }
                productoEncontrado.Stock -= cantidad;
            }
            productoEncontrado.UpdatedAt = DateTime.UtcNow;

            await _unidadDeTrabajo.GuardarCambiosAsync();
            _memoryCache.Remove(ProductosListCacheKey);
            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "OperacionStock", "OK", usuarioId, productoId);
            return Result.Success();
        }
        public async Task<Result> EliminarProductoAsync(int usuarioId, int productoId)
        {
            if (productoId <= 0)
            {
                return Result.Failure($"Su productoId no puede ser menor o igual a 0");
            }

            var usuarioEncontrado = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuarioEncontrado == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "EliminarProducto", "USUARIO_NO_EXISTE", usuarioId, productoId);
                return Result.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            var productoEncontrado = await _productoRepository.ObtenerProductoPorIdAsync(productoId);

            if (productoEncontrado == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "EliminarProducto", "PRODUCTO_NO_EXISTE", usuarioId, productoId);
                return Result.Failure($"Su producto con id = {productoId} no existe");
            }

            if (productoEncontrado.IsActive == false)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "EliminarProducto", "YA_ELIMINADO", usuarioId, productoId);
                return Result.Failure($"Su producto con id = {productoId} ya fue eliminado");
            }


            productoEncontrado.IsActive = false;
            productoEncontrado.UpdatedAt = DateTime.UtcNow;

            await _unidadDeTrabajo.GuardarCambiosAsync();
            _memoryCache.Remove(ProductosListCacheKey);
            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "EliminarProducto", "OK", usuarioId, productoId);
            return Result.Success();
        }
        public async Task<Result> ActualizarProductoAsync(int usuarioId, int productoId, ProductoActualizarDto productoActualizarDto)
        {
            if (productoId <= 0)
            {
                return Result.Failure($"Su productoId no debe de ser menor o igual a 0");
            }

            if (productoActualizarDto.Price <= 0)
            {
                return Result.Failure("El precio del producto no puede ser menor o igual a 0");
            }

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuario == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ActualizarProducto", "USUARIO_NO_EXISTE", usuarioId, productoId);
                return Result.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            var producto = await _productoRepository.ObtenerProductoPorIdAsync(productoId);

            if (producto == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ActualizarProducto", "PRODUCTO_NO_EXISTE", usuarioId, productoId);
                return Result.Failure($"Su producto con id = {productoId} no existe");
            }

            if (!producto.IsActive)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ActualizarProducto", "PRODUCTO_INACTIVO", usuarioId, productoId);
                return Result.Failure($"Su producto con id = {productoId} esta desactivado");
            }

            var categoriaEncontrada = await _categoriaRepository.ObtenerCategoriaPorIdAsync(productoActualizarDto.CategoryId);

            if (categoriaEncontrada == null)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ActualizarProducto", "CATEGORIA_NO_EXISTE", usuarioId, productoId);
                return Result.Failure($"Su categoria con id = {productoActualizarDto.CategoryId} no existe");
            }

            if (!categoriaEncontrada.IsActive)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ActualizarProducto", "CATEGORIA_INACTIVA", usuarioId, productoId);
                return Result.Failure($"Su categoria con id = {productoActualizarDto.CategoryId} esta desactivado");
            }

            var productoNombreNormalizado = productoActualizarDto.Name.Trim().ToLower();
            var existe = await _productoRepository.ExisteProductoConNombreEnCategoriaAsync(productoNombreNormalizado, productoActualizarDto.CategoryId, productoId);

            if (existe)
            {
                _logger.LogWarning("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ActualizarProducto", "NOMBRE_DUPLICADO", usuarioId, productoId);
                return Result.Failure("No puede existir dos productos con nombres igualas en la misma categoria");
            }

            producto.Name = productoNombreNormalizado;
            producto.Description = productoActualizarDto.Description;
            producto.CategoryId = productoActualizarDto.CategoryId;
            producto.Price = productoActualizarDto.Price;
            producto.UpdatedAt = DateTime.UtcNow;

            await _unidadDeTrabajo.GuardarCambiosAsync();
            _memoryCache.Remove(ProductosListCacheKey);
            _logger.LogInformation("Accion={Action} Resultado={Resultado} UserId={UserId} ProductId={ProductId}", "ActualizarProducto", "OK", usuarioId, productoId);
            return Result.Success();
        }
    }
}
