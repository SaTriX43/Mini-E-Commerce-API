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

        public ProductoService(IProductoRepository productoRepository, IUsuarioRepository usuarioRepository, ICategoriaRepository categoriaRepository)
        {
            _productoRepository = productoRepository;
            _usuarioRepository = usuarioRepository;
            _categoriaRepository = categoriaRepository;
        }

        public async Task<Result<ProductoDto>> CrearProductoAsync(ProductoCrearDto productoCrearDto, int usuarioId, string rol)
        {
            var usuarioExiste = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if (usuarioExiste == null) {
                return Result<ProductoDto>.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            if(rol != RolUsuario.Admin.ToString())
            {
                return Result<ProductoDto>.Failure($"No puede realizar esta accion ya que no es un Administrador");
            }

            if(productoCrearDto.Stock <= 0)
            {
                return Result<ProductoDto>.Failure($"No puede realizar esta accion ya que el stock debe de ser mayor a 0");
            }

            var categoriaExiste = await _categoriaRepository.ObtenerCategoriaPorIdAsync(productoCrearDto.CategoryId);

            if(categoriaExiste == null)
            {
                return Result<ProductoDto>.Failure($"Su categoria con id = {productoCrearDto.CategoryId} no existe");
            }

            if (!categoriaExiste.IsActive)
            {
                return Result<ProductoDto>.Failure($"Su categoria con id = {productoCrearDto.CategoryId} esta inactiva");
            }


            if (productoCrearDto.Price <= 0)
            {
                return Result<ProductoDto>.Failure($"El precio no puede ser menor o igual a 0");
            }

            var productoNombreNormzalizado = productoCrearDto.Name.Trim().ToLower();

            var existe = await _productoRepository.ExisteProductoConNombreEnCategoriaAsync(productoNombreNormzalizado,productoCrearDto.CategoryId);

            if (existe)
            {
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

            var productoCreado = await _productoRepository.CrearProductoAsync(productoModel);

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

            return Result<ProductoDto>.Success( productoDto );
        }

        public async Task<Result<ProductoDto>> ObtenerProductoPorIdAsdync(int usuarioId, int productoId)
        {
            var usuarioExise = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);

            if(usuarioExise == null)
            {
                return Result<ProductoDto>.Failure($"Su usuario con id = {usuarioId} no existe");
            }

            var productoExiste = await _productoRepository.ObtenerProductoPorIdAsync(productoId);
            
            if(productoExiste == null)
            {
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
    }
}
