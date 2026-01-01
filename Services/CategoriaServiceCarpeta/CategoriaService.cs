using Mini_E_Commerce_API.DALs.CategoriaRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.CategoriaDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.CategoriaServiceCarpeta
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public CategoriaService(ICategoriaRepository categoriaRepository, IUsuarioRepository usuarioRepository) { 
            _categoriaRepository = categoriaRepository;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Result<CategoriaDto>> CrearCategoriaAsync(CategoriaCrearDto categoriaDto, int usuarioId)
        {
            var usuarioExiste = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);
            if (usuarioExiste == null)
            {
                return Result<CategoriaDto>.Failure($"Usuario con id = {usuarioId} no existe");
            }

            var nombreCategoriaNormalizado = categoriaDto.Name.Trim().ToLower();
            var categoriaExiste = await _categoriaRepository.ObtenerCategoriaPorNombreAsync(categoriaDto.Name);

            if(categoriaExiste != null)
            {
                return Result<CategoriaDto>.Failure("No pueden existir 2 categorias con el mismo nombre");
            }

            var categoriaModel = new Categoria
            {
                Name = nombreCategoriaNormalizado,
                Description = categoriaDto.Description,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            };

            var categoriaCreada = await _categoriaRepository.CrearCategoriaAsync(categoriaModel);

            var categoriaCreadaDto = new CategoriaDto
            {
                Id = categoriaCreada.Id,
                IsActive = categoriaCreada.IsActive,
                CreatedAt = categoriaCreada.CreatedAt,
                Description = categoriaCreada.Description,
                Name = categoriaCreada.Name,
            };

            return Result<CategoriaDto>.Success(categoriaCreadaDto);
        }
    }
}
