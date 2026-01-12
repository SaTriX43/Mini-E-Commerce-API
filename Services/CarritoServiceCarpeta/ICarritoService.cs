using Mini_E_Commerce_API.Common.Results;
using Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta;

namespace Mini_E_Commerce_API.Services.CarritoServiceCarpeta
{
    public interface ICarritoService
    {
        public Task<Result<CarritoDto>> ObtenerCarritoPorUsuarioIdAsync(int usuarioId);
        public Task<Result> AgregarCarritoItemAsync(CarritoItemAgregarDto itemAgregarDto, int usuarioId);
        public Task<Result> ActualizarCantidadCarritoItemAsync(CarritoItemAgregarDto itemAgregarDto, int usuarioId);
        public Task<Result> EliminarCarritoItemAsync(int carritoItemId, int usuarioId);
        public Task<Result> VaciarCarritoAsync(int usuarioId);
    }
}
