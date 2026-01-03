using Mini_E_Commerce_API.DTOs.OrdenDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.OrdenServiceCarpeta
{
    public interface IOrdenService
    {
        public Task<Result<OrdenDto>> CrearOrdenAsync(int usuarioId);
        public Task<Result<List<OrdenDto>>> ObtenerOrdenesAsync(int usuarioId);
        public Task<Result<OrdenDto>> ObtenerOrdenDetallesAsync(int ordenId, int usuarioId);
        public Task<Result> CancelarOrdenAsync(int ordenId, int usuarioId);
        public Task<Result<List<OrdenDto>>> ObtenerTodasLasOrdenesAsync();
    }
}
