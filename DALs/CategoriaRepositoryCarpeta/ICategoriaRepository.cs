using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.CategoriaRepositoryCarpeta
{
    public interface ICategoriaRepository
    {
        public Task<Categoria?> ObtenerCategoriaPorIdAsync(int categoriaId);
        public Task<bool> ExisteCategoriaConNombreAsync(string categoriaNombre, int? categoriaIdExcluir);
        public Categoria CrearCategoria(Categoria categoria);
        public Task<List<Categoria>> ObtenerCategoriasAsync(bool soloActivas);
    }
}
