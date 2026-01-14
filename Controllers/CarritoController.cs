using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta;
using Mini_E_Commerce_API.Services.CarritoServiceCarpeta;
using System.Security.Claims;
using Mini_E_Commerce_API.Common.Responses;

namespace Mini_E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : BaseApiController
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService) { 
            _carritoService = carritoService;
        }

        /// <summary>
        /// Obtiene el carrito del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Si el usuario no tiene carrito, el sistema crea uno automáticamente.
        /// Requiere autenticación.
        /// </remarks>
        /// <response code="200">Carrito del usuario.</response>
        /// <response code="400">El usuarioId del token no es válido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Usuario no existe.</response>
        [ProducesResponseType(typeof(ApiResponseDto<CarritoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerCarritoUsuario()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var carrito = await _carritoService.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            return HandleResult(carrito);
        }


        /// <summary>
        /// Agrega un item al carrito del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Si el item ya existe en el carrito, se incrementa la cantidad.
        /// </remarks>
        /// <param name="itemAgregarDto">ProductoId y cantidad.</param>
        /// <response code="200">Item agregado correctamente.</response>
        /// <response code="400">Validación fallida (cantidad inválida o productId inválido).</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Usuario o producto no existe.</response>
        /// <response code="409">Producto inactivo o stock insuficiente.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [Authorize]
        [HttpPost("agregar-item")]
        public async Task<IActionResult> AgregarCarritoItem([FromBody] CarritoItemAgregarDto itemAgregarDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var resultado = await _carritoService.AgregarCarritoItemAsync(itemAgregarDto, usuarioId);

            return HandleResult(resultado);
        }


        /// <summary>
        /// Actualiza la cantidad de un item del carrito.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Si el item no existe en el carrito se devuelve error.
        /// </remarks>
        /// <param name="itemAgregarDto">ProductoId y nueva cantidad.</param>
        /// <response code="200">Cantidad actualizada correctamente.</response>
        /// <response code="400">Validación fallida (cantidad inválida o productId inválido).</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Usuario, producto o item no existe.</response>
        /// <response code="409">Producto inactivo o stock insuficiente.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [Authorize]
        [HttpPatch("actualizar-cantidad-item")]
        public async Task<IActionResult> ActualizarCantidadCarritoItem([FromBody] CarritoItemAgregarDto itemAgregarDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var resultado = await _carritoService.ActualizarCantidadCarritoItemAsync(itemAgregarDto, usuarioId);

            return HandleResult(resultado);
        }


        /// <summary>
        /// Elimina un item del carrito del usuario.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Solo permite eliminar items que pertenezcan al carrito del usuario autenticado.
        /// </remarks>
        /// <param name="carritoItemId">Id del item del carrito.</param>
        /// <response code="200">Item eliminado correctamente.</response>
        /// <response code="400">carritoItemId inválido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">El item no pertenece al usuario.</response>
        /// <response code="404">Usuario o item no existe.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize]
        [HttpDelete("eliminar-item/{carritoItemId}")]
        public async Task<IActionResult> EliminarCarritoItem(int carritoItemId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var resultado = await _carritoService.EliminarCarritoItemAsync(carritoItemId, usuarioId);

            return HandleResult(resultado);
        }


        /// <summary>
        /// Vacía el carrito del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Elimina todos los items del carrito.
        /// </remarks>
        /// <response code="200">Carrito vaciado correctamente.</response>
        /// <response code="400">El usuarioId del token no es válido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Usuario o carrito no existe.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpDelete("vaciar")]
        public async Task<IActionResult> VaciarCarrito()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var resultado = await _carritoService.VaciarCarritoAsync(usuarioId);

            return HandleResult(resultado);
        }

    }
}
