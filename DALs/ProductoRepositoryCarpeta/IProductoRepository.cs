using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta
{
    public interface IProductoRepository
    {
        public Producto CrearProducto(Producto producto);
        public Task<Producto?> ObtenerProductoPorIdAsync(int productoId);
        public Task<List<Producto>> ObtenerProductosAsync();
        public Task<bool> ExisteProductoConNombreEnCategoriaAsync(string productoNombre, int categoriaId, int? productoIdExcluir);
    }
}
