using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Common.Responses;
using Mini_E_Commerce_API.DTOs.OrdenDtoCarpeta;
using Mini_E_Commerce_API.Services.OrdenServiceCarpeta;

namespace Mini_E_Commerce_API.Controllers.OrdenesControllerCarpeta
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesController : BaseApiController
    {
        private readonly IOrdenService _ordenService;

        public OrdenesController(IOrdenService ordenService)
        {
            _ordenService = ordenService;
        }

        /// <summary>
        /// Crea una nueva orden con los items del carrito del usuario.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// <br/><br/>
        /// Reglas de negocio:
        /// - El carrito debe existir
        /// - El carrito no puede estar vacío
        /// - Todos los productos deben existir y estar activos
        /// - Debe haber stock suficiente
        /// <br/><br/>
        /// La orden se crea con estado <b>Pending</b>.
        /// </remarks>
        /// <response code="200">Orden creada correctamente.</response>
        /// <response code="400">usuarioId inválido en el token.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Carrito no existe o producto no existe.</response>
        /// <response code="409">Carrito vacío o stock insuficiente.</response>
        [ProducesResponseType(typeof(ApiResponseDto<OrdenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearOrden()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var orden = await _ordenService.CrearOrdenAsync(usuarioId);

            return HandleResult(orden);
        }

        /// <summary>
        /// Obtiene las órdenes del usuario autenticado.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Devuelve todas las órdenes asociadas al usuario.
        /// </remarks>
        /// <response code="200">Listado de órdenes.</response>
        /// <response code="400">usuarioId inválido en el token.</response>
        /// <response code="401">No autenticado.</response>
        [ProducesResponseType(typeof(ApiResponseDto<List<OrdenDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenes()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var ordenes = await _ordenService.ObtenerOrdenesUsuarioAsync(usuarioId);

            return HandleResult(ordenes);
        }

        /// <summary>
        /// Obtiene el detalle de una orden específica del usuario.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Solo devuelve la orden si pertenece al usuario autenticado.
        /// </remarks>
        /// <param name="ordenId">Id de la orden.</param>
        /// <response code="200">Detalle de la orden.</response>
        /// <response code="400">ordenId inválido o usuarioId inválido en token.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Orden no existe o no pertenece al usuario.</response>
        [ProducesResponseType(typeof(ApiResponseDto<OrdenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet("{ordenId}")]
        public async Task<IActionResult> ObtenerOrdenDetallesPorId(int ordenId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var ordenDetalles = await _ordenService.ObtenerOrdenDetallesUsuarioAsync(ordenId, usuarioId);

            return HandleResult(ordenDetalles);
        }

        /// <summary>
        /// Cancela una orden pendiente del usuario.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// <br/><br/>
        /// Reglas:
        /// - Solo se puede cancelar si la orden está en estado <b>Pending</b>
        /// - No se puede cancelar si ya está cancelada
        /// - No se puede cancelar si está pagada u otro estado
        /// </remarks>
        /// <param name="ordenId">Id de la orden.</param>
        /// <response code="200">Orden cancelada correctamente.</response>
        /// <response code="400">ordenId inválido o usuarioId inválido en token.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Orden no existe o no pertenece al usuario.</response>
        /// <response code="409">Conflicto: ya cancelada o estado inválido para cancelar.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [Authorize]
        [HttpPost("cancelar/{ordenId}")]
        public async Task<IActionResult> CancelarOrden(int ordenId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var resultado = await _ordenService.CancelarOrdenAsync(ordenId, usuarioId);

            return HandleResult(resultado);
        }

        /// <summary>
        /// Paga una orden pendiente del usuario.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// <br/><br/>
        /// Reglas:
        /// - Solo se puede pagar si la orden está en <b>Pending</b>
        /// - Valida stock antes de pagar
        /// - Descuenta stock
        /// - Vacía el carrito
        /// - Cambia estado a <b>Paid</b>
        /// </remarks>
        /// <param name="ordenId">Id de la orden.</param>
        /// <response code="200">Orden pagada correctamente.</response>
        /// <response code="400">ordenId inválido o usuarioId inválido en token.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Orden no existe o no pertenece al usuario.</response>
        /// <response code="409">Conflicto: estado inválido o stock insuficiente.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [Authorize]
        [HttpPost("pagar/{ordenId}")]
        public async Task<IActionResult> PagarOrden(int ordenId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var resultado = await _ordenService.PagarOrdenAsync(ordenId, usuarioId);

            return HandleResult(resultado);
        }
    }
}
