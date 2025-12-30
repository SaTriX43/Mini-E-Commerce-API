using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta
{
    public interface IProductoRepository
    {
        public Task<Producto> CrearProductoAsync(Producto producto);
        public Task<Producto?> ObtenerProductoPorIdAsync(int productoId);
        public Task<bool> ExisteProductoConNombreEnCategoriaAsync(string productoNombre, int categoriaId);
    }
}
