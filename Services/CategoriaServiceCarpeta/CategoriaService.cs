using Microsoft.Extensions.Caching.Memory;
using Mini_E_Commerce_API.DALs;
using Mini_E_Commerce_API.DALs.CategoriaRepositoryCarpeta;
using Mini_E_Commerce_API.DALs.UsuariorRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.CategoriaDtoCarpeta;
using Mini_E_Commerce_API.Models;
using Mini_E_Commerce_API.Models.Enums;

namespace Mini_E_Commerce_API.Services.CategoriaServiceCarpeta
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnidadDeTrabajo _unidadDeTrabajo;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CategoriaService> _logger;
        private readonly IMemoryCache _memoryCache;
        private const string CategoriasListCacheKey = "categorias:key";

        public CategoriaService(ICategoriaRepository categoriaRepository, IUsuarioRepository usuarioRepository, IUnidadDeTrabajo unidadDeTrabajo, IConfiguration configuration, ILogger<CategoriaService> logger, IMemoryCache memoryCache) { 
            _categoriaRepository = categoriaRepository;
            _usuarioRepository = usuarioRepository;
            _unidadDeTrabajo = unidadDeTrabajo;
            _configuration = configuration;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<Result<CategoriaDto>> CrearCategoriaAsync(CategoriaCrearDto categoriaDto, int usuarioId)
        {
            var usuarioExiste = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);
            if (usuarioExiste == null)
            {
                return Result<CategoriaDto>.Failure($"Usuario con id = {usuarioId} no existe");
            }

            var nombreCategoriaNormalizado = categoriaDto.Name.Trim().ToLower();
            var categoriaExiste = await _categoriaRepository.ExisteCategoriaConNombreAsync(categoriaDto.Name, null);

            if(categoriaExiste)
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

            var categoriaCreada = _categoriaRepository.CrearCategoria(categoriaModel);

            var categoriaCreadaDto = new CategoriaDto
            {
                Id = categoriaCreada.Id,
                IsActive = categoriaCreada.IsActive,
                CreatedAt = categoriaCreada.CreatedAt,
                Description = categoriaCreada.Description,
                Name = categoriaCreada.Name,
            };

            await _unidadDeTrabajo.GuardarCambiosAsync();
            _memoryCache.Remove(CategoriasListCacheKey);
            return Result<CategoriaDto>.Success(categoriaCreadaDto);
        }
        public async Task<Result> ActualizarCategoriaAsync(
            CategoriaCrearDto dto,
            int categoriaId,
            int usuarioId
            )
        {
            if (categoriaId <= 0)
                return Result.Failure("El categoriaId debe ser mayor a 0");

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);
            if (usuario == null)
                return Result.Failure($"El usuario con id {usuarioId} no existe");

            if (usuario.Rol != RolUsuario.Admin)
                return Result.Failure("No tiene permisos para actualizar categorías");

            var categoria = await _categoriaRepository.ObtenerCategoriaPorIdAsync(categoriaId);
            if (categoria == null)
                return Result.Failure($"La categoría con id {categoriaId} no existe");

            if (!categoria.IsActive)
                return Result.Failure("No se puede actualizar una categoría eliminada");

            var nombreNormalizado = dto.Name.Trim().ToLower();

            var nombreExiste = await _categoriaRepository
                .ExisteCategoriaConNombreAsync(nombreNormalizado, categoriaId);

            if (nombreExiste)
                return Result.Failure("Ya existe una categoría con ese nombre");

            categoria.Name = nombreNormalizado;
            categoria.Description = dto.Description;

            await _unidadDeTrabajo.GuardarCambiosAsync();
            _memoryCache.Remove(CategoriasListCacheKey);

            return Result.Success();
        }

        public async Task<Result> DesactivarCategoriaAsync(int usuarioId, int categoriaId)
        {
            if (categoriaId <= 0)
                return Result.Failure("El categoriaId debe ser mayor a 0");

            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);
            if (usuario == null)
                return Result.Failure($"El usuario con id {usuarioId} no existe");

            if (usuario.Rol != RolUsuario.Admin)
                return Result.Failure("No tiene permisos para desactivar categorías");

            var categoria = await _categoriaRepository.ObtenerCategoriaPorIdAsync(categoriaId);
            if (categoria == null)
                return Result.Failure($"La categoría con id {categoriaId} no existe");

            if (!categoria.IsActive)
                return Result.Failure("La categoría ya se encuentra desactivada");

            categoria.IsActive = false;

            await _unidadDeTrabajo.GuardarCambiosAsync();

            _memoryCache.Remove(CategoriasListCacheKey);

            return Result.Success();
        }

        public async Task<Result<List<CategoriaDto>>> ObtenerCategoriasAsync(int usuarioId)
        {
            var usuario = await _usuarioRepository.ObtenerUsuarioPorIdAsync(usuarioId);
            if (usuario == null)
                return Result<List<CategoriaDto>>.Failure("El usuario no existe");

            bool soloActivas = usuario.Rol != RolUsuario.Admin;

            if(_memoryCache.TryGetValue(CategoriasListCacheKey, out List<CategoriaDto>? cached))
            {
                _logger.LogInformation("Categorias de [CACHE]");
                return Result<List<CategoriaDto>>.Success(cached!);
            }

            var categorias = await _categoriaRepository.ObtenerCategoriasAsync(soloActivas);

            var dto = categorias.Select(c => new CategoriaDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive
            }).ToList();

            var segundoExpiracionCategoriaCache = _configuration.GetValue<int>("ConfigCache:DuracionCacheSegundos");

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(segundoExpiracionCategoriaCache)
            };

            _memoryCache.Set(CategoriasListCacheKey, dto, options);

            return Result<List<CategoriaDto>>.Success(dto);
        }
    }
}

