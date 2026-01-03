using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.OrdenRepositoryCarpeta
{
    public class OrdenRepository : IOrdenRepository
    {
        private readonly ApplicationDbContext _context;

        public OrdenRepository(ApplicationDbContext context) { 
            _context = context;
        }

        public Orden CrearOrden(Orden orden)
        {
            _context.Ordenes.Add(orden);
            return orden;
        }

        public async Task<List<Orden>> ObtenerOrdenesPorUsuarioIdAsync(int usuarioId)
        {
            var ordenes = await _context.Ordenes
                .Include(o => o.Detalles)
                .Where(o => o.UserId == usuarioId)
                .ToListAsync();

            return ordenes;
        }
    }
}
