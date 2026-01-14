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
    public class AdminOrdenesController : BaseApiController
    {
        private readonly IOrdenService _ordenService;

        public AdminOrdenesController(IOrdenService ordenService)
        {
            _ordenService = ordenService;
        }

        /// <summary>
        /// Obtiene todas las órdenes del sistema.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// Devuelve todas las órdenes de todos los usuarios.
        /// </remarks>
        /// <response code="200">Listado de órdenes.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        [ProducesResponseType(typeof(ApiResponseDto<List<OrdenDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenesUsuarios()
        {
            var ordenes = await _ordenService.ObtenerTodasLasOrdenesAsync();

            return HandleResult(ordenes);
        }

        /// <summary>
        /// Obtiene todas las órdenes de un usuario específico.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// Valida que el usuario exista.
        /// </remarks>
        /// <param name="usuarioId">Id del usuario.</param>
        /// <response code="200">Listado de órdenes del usuario.</response>
        /// <response code="400">usuarioId inválido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Usuario no existe.</response>
        [ProducesResponseType(typeof(ApiResponseDto<List<OrdenDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpGet("ordenes-usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerOrdenesUsuarioPorId(int usuarioId)
        {
            var ordenes = await _ordenService.ObtenerOrdenesPorUsuarioAdminAsync(usuarioId);

            return HandleResult(ordenes);
        }

        /// <summary>
        /// Obtiene el detalle de una orden por Id (modo Admin).
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// Devuelve el detalle de la orden aunque no pertenezca al Admin.
        /// </remarks>
        /// <param name="ordenId">Id de la orden.</param>
        /// <response code="200">Detalle de la orden.</response>
        /// <response code="400">ordenId inválido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Orden no existe.</response>
        [ProducesResponseType(typeof(ApiResponseDto<OrdenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpGet("orden-detalles/{ordenId}")]
        public async Task<IActionResult> ObtenerOrdenDetallesPorId(int ordenId)
        {
            var ordenes = await _ordenService.ObtenerOrdenDetallesUsuarioAdminAsync(ordenId);

            return HandleResult(ordenes);
        }
    }
}
