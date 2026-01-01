using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.CategoriaRepositoryCarpeta
{
    public interface ICategoriaRepository
    {
        public Task<Categoria?> ObtenerCategoriaPorIdAsync(int categoriaId);
        public Task<Categoria?> ObtenerCategoriaPorNombreAsync(string categoriaNombre);
        public Task<Categoria> CrearCategoriaAsync(Categoria categoria);
    }
}
