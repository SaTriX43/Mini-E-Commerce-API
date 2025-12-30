using Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta;
using Mini_E_Commerce_API.Models;

namespace Mini_E_Commerce_API.Services.ProductoServiceCarpeta
{
    public interface IProductoService
    {
        public Task<Result<ProductoDto>> CrearProductoAsync(ProductoCrearDto productoCrearDto, int usuarioId, string rol);
        public Task<Result<ProductoDto>> ObtenerProductoPorIdAsdync(int usuarioId,int productoId);
    }
}
