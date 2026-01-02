using Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.CarritoServiceCarpeta
{
    public interface ICarritoService
    {
        public Task<Result<CarritoDto>> ObtenerCarritoPorUsuarioIdAsync(int usuarioId);
        public Task<Result> AgregarCarritoItemAsync(CarritoItemAgregarDto itemAgregarDto, int usuarioId);
        public Task<Result> ActualizarCantidadCarritoItemAsync(CarritoItemAgregarDto itemAgregarDto, int usuarioId);
        public Task<Result> EliminarCarritoItemAsync(int carritoItemId, int usuarioId);
    }
}
