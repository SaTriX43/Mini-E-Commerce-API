using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Common.Responses;
using Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta;
using Mini_E_Commerce_API.Models.Enums;
using Mini_E_Commerce_API.Services.ProductoServiceCarpeta;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : BaseApiController
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService) { 
            _productoService = productoService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoCrearDto productoCrearDto)
        {
           if(!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var productoCreado = await _productoService.CrearProductoAsync(productoCrearDto,usuarioId);

            return HandleResult(productoCreado);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("incrementar-stock/{productoId}")]
        public async Task<IActionResult> IncrementarStockProducto([FromBody] StockDto stock, int productoId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultadoIncrementar = await _productoService.OperacionStockProductoAsync(usuarioId, productoId,TipoDeMovimiento.Incrementar,stock.Cantidad);

            return HandleResult(resultadoIncrementar);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("disminuir-stock/{productoId}")]
        public async Task<IActionResult> DisminuirStockProducto([FromBody] StockDto stock, int productoId)
        {

            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultadoIncrementar = await _productoService.OperacionStockProductoAsync(usuarioId, productoId, TipoDeMovimiento.Disminuir, stock.Cantidad);

            return HandleResult(resultadoIncrementar);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{productoId}")]
        public async Task<IActionResult> EliminarProducto(int productoId)
        {
           if(!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var productoEliminado = await _productoService.EliminarProductoAsync(usuarioId, productoId);

            return HandleResult(productoEliminado);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{productoId}")]
        public async Task<IActionResult> ActualizarProducto(int productoId, [FromBody] ProductoActualizarDto productoActualizarDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var productoActualizado = await _productoService.ActualizarProductoAsync(usuarioId, productoId,productoActualizarDto);

            return HandleResult(productoActualizado);
        }

        [ProducesResponseType(typeof(ApiResponseDto<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>),StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet("{productoId}")]
        public async Task<IActionResult> ObtenerProductoPorId(int productoId)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(productoId);

            return HandleResult(producto);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerProductos()
        {
            var productos = await _productoService.ObtenerProductosAsync();

            return HandleResult(productos);
        }





    }
}
