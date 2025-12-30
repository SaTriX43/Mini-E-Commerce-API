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
    }
}
