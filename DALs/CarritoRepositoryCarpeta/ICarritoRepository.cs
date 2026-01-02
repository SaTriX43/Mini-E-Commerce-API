using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.CarritoRepositoryCarpeta
{
    public interface ICarritoRepository
    {
        public Task<Carrito?> ObtenerCarritoPorUsuarioIdAsync(int usuarioId);
        public Task<Carrito> CrearCarritoAsync(Carrito carrito);
    }
}
