using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.CategoriaRepositoryCarpeta
{
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoriaRepository(ApplicationDbContext context) { 
            _context = context;
        }

        public async Task<Categoria?> ObtenerCategoriaPorIdAsync(int categoriaId)
        {
            var categoriaEcontrada = await _context.Categorias.FirstOrDefaultAsync(c => c.Id == categoriaId);
            return categoriaEcontrada;
        }

        public async Task<Categoria?> ObtenerCategoriaPorNombreAsync(string categoriaNombre)
        {
            var categoriaEncontrada = await _context.Categorias.FirstOrDefaultAsync(c => c.Name == categoriaNombre);
            return categoriaEncontrada;
        }

        public async Task<Categoria> CrearCategoriaAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }
    }
}
