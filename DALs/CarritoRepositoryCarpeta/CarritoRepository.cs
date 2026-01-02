using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.CarritoRepositoryCarpeta
{
    public class CarritoRepository : ICarritoRepository
    {
        private readonly ApplicationDbContext _context;

        public CarritoRepository(ApplicationDbContext context) { 
            _context = context;
        }

        public async Task<Carrito?> ObtenerCarritoPorUsuarioIdAsync(int usuarioId)
        {
            var carrito = await _context.Carritos
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Producto)
                .FirstOrDefaultAsync(c => c.UserId == usuarioId);
            return carrito;
        }

        public async Task<Carrito> CrearCarritoAsync(Carrito carrito)
        {
            _context.Carritos.Add(carrito);
            await _context.SaveChangesAsync();
            return carrito;
        }
    }
}
