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

        public async Task<bool> ExisteCategoriaConNombreAsync(string categoriaNombre, int? categoriaIdExcluir)
        {
            if(categoriaIdExcluir.HasValue)
            {
                return await _context.Categorias.AnyAsync(c =>
                    c.Name == categoriaNombre &&
                    c.Id != categoriaIdExcluir &&
                    c.IsActive
                );
            }

            return await _context.Categorias.AnyAsync(c =>
                   c.Name == categoriaNombre &&
                   c.IsActive
               );
        }

        public async Task<Categoria> CrearCategoriaAsync(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();
            return categoria;
        }

        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
