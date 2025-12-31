using Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta;
using Mini_E_Commerce_API.Models;
using Mini_E_Commerce_API.Models.Enums;

namespace Mini_E_Commerce_API.Services.ProductoServiceCarpeta
{
    public interface IProductoService
    {
        public Task<Result<ProductoDto>> CrearProductoAsync(ProductoCrearDto productoCrearDto, int usuarioId, string rol);
        public Task<Result<ProductoDto>> ObtenerProductoPorIdAsync(int usuarioId,int productoId);
        public Task<Result<List<ProductoDto>>> ObtenerProductosAsync(int usuarioId);
        public Task<Result> OperacionStockProductoAsync(int usuarioId, int productoId, TipoDeMovimiento tipoDeMovimiento, int cantidad);
        public Task<Result> EliminarProductoAsync(int usuarioId,int productoId);
        public Task<Result> ActualizarProductoAsync(int usuarioId, int productoId, ProductoActualizarDto productoActualizarDto);
    }
}
