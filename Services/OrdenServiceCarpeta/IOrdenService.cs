using Mini_E_Commerce_API.DTOs.OrdenDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.OrdenServiceCarpeta
{
    public interface IOrdenService
    {
        //usuario
        public Task<Result<OrdenDto>> CrearOrdenAsync(int usuarioId);
        public Task<Result<List<OrdenDto>>> ObtenerOrdenesUsuarioAsync(int usuarioId);
        public Task<Result<OrdenDto>> ObtenerOrdenDetallesUsuarioAsync(int ordenId, int usuarioId);
        public Task<Result> CancelarOrdenAsync(int ordenId, int usuarioId);


        //admin

        public Task<Result<List<OrdenDto>>> ObtenerTodasLasOrdenesAsync();
        public Task<Result<List<OrdenDto>>> ObtenerOrdenesPorUsuarioAdminAsync(int usuarioId);
        public Task<Result<OrdenDto>> ObtenerOrdenDetallesUsuarioAdminAsync(int ordenId);
    }
}
