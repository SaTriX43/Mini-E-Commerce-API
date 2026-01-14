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

        /// <summary>
        /// Crea un producto.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// </remarks>
        /// <param name="productoCrearDto">Datos del producto a crear.</param>
        /// <response code="200">Producto creado correctamente.</response>
        /// <response code="400">Validación fallida (stock o precio inválido).</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Usuario o categoría no existe.</response>
        /// <response code="409">Conflicto: categoría inactiva o producto duplicado.</response>
        [ProducesResponseType(typeof(ApiResponseDto<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoCrearDto productoCrearDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var productoCreado = await _productoService.CrearProductoAsync(productoCrearDto, usuarioId);

            return HandleResult(productoCreado);
        }

        /// <summary>
        /// Endpoint para Incrementar stock
        /// </summary>
        /// <remarks>
        ///     Sirve para incrementar el stock en base a movimientos
        ///     Requiere rol Admin
        /// </remarks>
        /// <param name="stock">Cantidad</param>
        /// <param name="productoId">ProductoId</param>
        /// <response code="200">Stock incrementado correctamente.</response>
        /// <response code="400">productoId inválido o cantidad inválida.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Producto no existe.</response>
        /// <response code="409">Producto inactivo o conflicto de negocio.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
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


        /// <summary>
        /// Disminuye el stock de un producto mediante un movimiento.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// Valida que exista stock suficiente.
        /// </remarks>
        /// <param name="stock">Cantidad a disminuir (debe ser mayor a 0).</param>
        /// <param name="productoId">Id del producto.</param>
        /// <response code="200">Stock actualizado correctamente.</response>
        /// <response code="400">productoId inválido o cantidad inválida.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Usuario o producto no existe.</response>
        /// <response code="409">Producto inactivo o stock insuficiente.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpPatch("disminuir-stock/{productoId}")]
        public async Task<IActionResult> DisminuirStockProducto([FromBody] StockDto stock, int productoId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var resultadoDisminuir = await _productoService.OperacionStockProductoAsync(
                usuarioId,
                productoId,
                TipoDeMovimiento.Disminuir,
                stock.Cantidad);

            return HandleResult(resultadoDisminuir);
        }

        /// <summary>
        /// Elimina (desactiva) un producto.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// La eliminación es lógica (soft delete): el producto queda inactivo.
        /// </remarks>
        /// <param name="productoId">Id del producto.</param>
        /// <response code="200">Producto desactivado correctamente.</response>
        /// <response code="400">productoId inválido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Usuario o producto no existe.</response>
        /// <response code="409">El producto ya estaba inactivo.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{productoId}")]
        public async Task<IActionResult> EliminarProducto(int productoId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var productoEliminado = await _productoService.EliminarProductoAsync(usuarioId, productoId);

            return HandleResult(productoEliminado);
        }


        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// </remarks>
        /// <param name="productoId">Id del producto.</param>
        /// <param name="productoActualizarDto">Datos actualizados.</param>
        /// <response code="200">Producto actualizado correctamente.</response>
        /// <response code="400">Validación fallida (productoId inválido o precio inválido).</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Usuario, producto o categoría no existe.</response>
        /// <response code="409">Conflicto: producto/categoría inactiva o nombre duplicado.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpPut("{productoId}")]
        public async Task<IActionResult> ActualizarProducto(int productoId, [FromBody] ProductoActualizarDto productoActualizarDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var productoActualizado = await _productoService.ActualizarProductoAsync(usuarioId, productoId, productoActualizarDto);

            return HandleResult(productoActualizado);
        }


        /// <summary>
        /// Obtiene un producto por su Id.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// </remarks>
        /// <param name="productoId">Id del producto.</param>
        /// <response code="200">Producto encontrado.</response>
        /// <response code="400">productoId inválido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Producto no existe.</response>
        [ProducesResponseType(typeof(ApiResponseDto<ProductoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("{productoId}")]
        public async Task<IActionResult> ObtenerProductoPorId(int productoId)
        {
            var producto = await _productoService.ObtenerProductoPorIdAsync(productoId);

            return HandleResult(producto);
        }


        /// <summary>
        /// Obtiene la lista de productos.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Puede devolver resultados desde caché o base de datos.
        /// </remarks>
        /// <response code="200">Lista de productos.</response>
        /// <response code="401">No autenticado.</response>
        [ProducesResponseType(typeof(ApiResponseDto<List<ProductoDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerProductos()
        {
            var productos = await _productoService.ObtenerProductosAsync();

            return HandleResult(productos);
        }






    }
}
