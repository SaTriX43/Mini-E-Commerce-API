using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;
using System.Reflection.Metadata.Ecma335;

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
        public async Task<bool> ExisteProductoConNombreEnCategoriaAsync(string productoNombre, int categoriaId)
        {

            return await _context.Productos.AnyAsync(p =>
                p.Name == productoNombre &&
                p.CategoryId == categoriaId &&
                p.IsActive
            );
        }
        public async Task<Producto?> ObtenerProductoPorIdAsync(int productoId)
        {
            var productoEncontrado = await _context.Productos.FirstOrDefaultAsync(p => p.Id == productoId);
            return productoEncontrado;
        }
        public async Task<List<Producto>> ObtenerProductosAsync()
        {
            var productos = await _context.Productos.ToListAsync();
            return productos;
        }
        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
