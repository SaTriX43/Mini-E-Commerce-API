using Mini_E_Commerce_API.DALs.CategoriaRepositoryCarpeta;
using Mini_E_Commerce_API.DTOs.CategoriaDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.CategoriaServiceCarpeta
{
    public interface ICategoriaService
    {
        public Task<Result<CategoriaDto>> CrearCategoriaAsync(CategoriaCrearDto categoriaDto, int usuarioId);
    }
}
