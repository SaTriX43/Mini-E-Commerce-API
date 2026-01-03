using Microsoft.EntityFrameworkCore;
using Mini_E_Commerce_API.Models;
using System.Threading.Tasks;

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

        public Carrito CrearCarrito(Carrito carrito)
        {
            _context.Carritos.Add(carrito);
            return carrito;
        }

        public void AgregarCarritoItem(CarritoItem carritoItem)
        {
            _context.CarritoItems.Add(carritoItem);
        }

        public async Task<CarritoItem?> ObtenerCarritoItemAsync(int carritoId, int productoId)
        {
            var carritoItem = await _context.CarritoItems.FirstOrDefaultAsync(ci => ci.ProductId == productoId && ci.CartId == carritoId);
            return carritoItem;
        }

        public void EliminarItemCarrito(CarritoItem carritoItem)
        {
            _context.CarritoItems.Remove(carritoItem);
        }

        public async Task<CarritoItem?> ObtenerCarritoItemPorIdAsync(int carritoItemId)
        {
            var carritoItem = await _context.CarritoItems
                .Include(ci => ci.Carrito)
                .FirstOrDefaultAsync(ci => ci.Id == carritoItemId);
            return carritoItem;
        }

        public async Task VaciarCarritoItems(int carritoId)
        {
            var items = await _context.CarritoItems.Where(ci => ci.CartId == carritoId).ToListAsync();

            _context.CarritoItems.RemoveRange(items);
        }
        public async Task GuardarCambiosAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
