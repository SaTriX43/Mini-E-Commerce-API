using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.CarritoRepositoryCarpeta
{
    public interface ICarritoRepository
    {
        public Task<Carrito?> ObtenerCarritoPorUsuarioIdAsync(int usuarioId);
        public Carrito CrearCarrito(Carrito carrito);
        public void AgregarCarritoItem(CarritoItem carritoItem);
        public Task<CarritoItem?> ObtenerCarritoItemAsync(int carritoId, int productoId);
        public void EliminarItemCarrito(CarritoItem carritoItem);
        public Task<CarritoItem?> ObtenerCarritoItemPorIdAsync(int carritoItemId);
        public Task VaciarCarritoItems(int carritoId);
    }
}
