using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.DALs.OrdenRepositoryCarpeta
{
    public interface IOrdenRepository
    {
        public Orden CrearOrden(Orden orden);
        public Task<List<Orden>> ObtenerOrdenesPorUsuarioIdAsync(int usuarioId);
        public Task<Orden?> ObtenerOrdenPorOrdenIdAsync(int ordenId);

    }
}
