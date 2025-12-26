using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.ProductoRepositoryCarpeta
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductoRepository(ApplicationDbContext context) { 
            _context = context;
        }

        public async Task<Producto> CrearProductoAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }
        public async Task<Producto?> ObtenerProductoPorNombre(string productoNombre)
        {
            var productoEncontrado = await _context.Productos.FirstOrDefaultAsync(p => p.Name == productoNombre);
            return productoEncontrado;
        }
    }
}
