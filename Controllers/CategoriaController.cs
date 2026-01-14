using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Common.Responses;
using Mini_E_Commerce_API.DTOs.CategoriaDtoCarpeta;
using Mini_E_Commerce_API.Services.CategoriaServiceCarpeta;

namespace Mini_E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : BaseApiController
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        /// <summary>
        /// Crea una categoría.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// Normaliza el nombre (trim + lower).
        /// </remarks>
        /// <param name="categoriaCrearDto">Datos de la categoría a crear.</param>
        /// <response code="200">Categoría creada correctamente.</response>
        /// <response code="400">El usuarioId del token no es válido.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No tiene rol Admin.</response>
        /// <response code="404">Usuario no existe.</response>
        /// <response code="409">Conflicto: ya existe una categoría con el mismo nombre.</response>
        [ProducesResponseType(typeof(ApiResponseDto<CategoriaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaCrearDto categoriaCrearDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var categoria = await _categoriaService.CrearCategoriaAsync(categoriaCrearDto, usuarioId);

            return HandleResult(categoria);
        }

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// No permite actualizar categorías inactivas.
        /// Valida duplicados por nombre.
        /// </remarks>
        /// <param name="categoriaId">Id de la categoría.</param>
        /// <param name="dto">Datos actualizados de la categoría.</param>
        /// <response code="200">Categoría actualizada correctamente.</response>
        /// <response code="400">categoriaId inválido o usuarioId inválido en token.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado (no Admin).</response>
        /// <response code="404">Usuario o categoría no existe.</response>
        /// <response code="409">Conflicto: categoría inactiva o nombre duplicado.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpPut("{categoriaId}")]
        public async Task<IActionResult> ActualizarCategoria(int categoriaId, [FromBody] CategoriaCrearDto dto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var result = await _categoriaService.ActualizarCategoriaAsync(dto, categoriaId, usuarioId);

            return HandleResult(result);
        }

        /// <summary>
        /// Desactiva (elimina lógicamente) una categoría.
        /// </summary>
        /// <remarks>
        /// Requiere rol <b>Admin</b>.
        /// La eliminación es lógica (soft delete): marca IsActive=false.
        /// </remarks>
        /// <param name="categoriaId">Id de la categoría.</param>
        /// <response code="200">Categoría desactivada correctamente.</response>
        /// <response code="400">categoriaId inválido o usuarioId inválido en token.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="403">No autorizado (no Admin).</response>
        /// <response code="404">Usuario o categoría no existe.</response>
        /// <response code="409">La categoría ya estaba inactiva.</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoriaId}")]
        public async Task<IActionResult> DesactivarCategoria(int categoriaId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var result = await _categoriaService.DesactivarCategoriaAsync(usuarioId, categoriaId);

            return HandleResult(result);
        }

        /// <summary>
        /// Obtiene categorías disponibles.
        /// </summary>
        /// <remarks>
        /// Requiere autenticación.
        /// Si el usuario no es Admin, devuelve solo categorías activas.
        /// Puede responder desde caché o base de datos.
        /// </remarks>
        /// <response code="200">Listado de categorías.</response>
        /// <response code="400">usuarioId inválido en el token.</response>
        /// <response code="401">No autenticado.</response>
        /// <response code="404">Usuario no existe.</response>
        [ProducesResponseType(typeof(ApiResponseDto<List<CategoriaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
                return error;

            var result = await _categoriaService.ObtenerCategoriasAsync(usuarioId);

            return HandleResult(result);
        }
    }
}
