using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Common.Responses;
using Mini_E_Commerce_API.DTOs.AutenticacionDtoCarpeta;
using Mini_E_Commerce_API.Services.AutenticacionServiceCarpeta;

namespace Mini_E_Commerce_API.Controllers
{
    [ApiController]
    [Route("api/autenticacion")]
    public class AutenticacionController : BaseApiController
    {
        private readonly IAutenticacionService _autenticacionService;

        public AutenticacionController(IAutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
        }

        /// <summary>
        /// Registra un usuario nuevo.
        /// </summary>
        /// <remarks>
        /// Crea un usuario con rol <b>User</b>, genera <b>AccessToken (JWT)</b> y <b>RefreshToken</b>.
        /// <br/><br/>
        /// <b>Seguridad:</b>
        /// - No loguear el token.
        /// - El refresh token se guarda en base de datos.
        /// </remarks>
        /// <param name="dto">Datos del registro (email, nombre y contraseña).</param>
        /// <response code="200">Usuario registrado correctamente. Devuelve AccessToken + RefreshToken.</response>
        /// <response code="400">Error de validación o email ya registrado.</response>
        [ProducesResponseType(typeof(ApiResponseDto<AutenticacionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [HttpPost("registro")]
        public async Task<IActionResult> Registrar([FromBody] RegistroRequestDto dto)
        {
            var result = await _autenticacionService.RegistrarAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// Inicia sesión.
        /// </summary>
        /// <remarks>
        /// Valida credenciales y devuelve:
        /// - <b>AccessToken</b> (JWT)
        /// - <b>RefreshToken</b>
        /// <br/><br/>
        /// Si las credenciales son incorrectas, devuelve un error estándar.
        /// </remarks>
        /// <param name="dto">Credenciales del usuario.</param>
        /// <response code="200">Login exitoso. Devuelve AccessToken + RefreshToken.</response>
        /// <response code="400">Credenciales inválidas.</response>
        [ProducesResponseType(typeof(ApiResponseDto<AutenticacionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _autenticacionService.LoginAsync(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// Renueva el access token usando refresh token.
        /// </summary>
        /// <remarks>
        /// Rotación de refresh token:
        /// - El token anterior pasa a estado <b>Used/Revoked</b>.
        /// - Se emite un nuevo RefreshToken.
        /// <br/><br/>
        /// Estados de error comunes:
        /// - Token no existe
        /// - Token expirado
        /// - Token revocado/usado
        /// - Usuario no existe asociado al token
        /// </remarks>
        /// <param name="dto">Refresh token válido.</param>
        /// <response code="200">Token renovado. Devuelve AccessToken + RefreshToken nuevo.</response>
        /// <response code="400">Token inválido (no existe, expirado, revocado, usado).</response>
        [ProducesResponseType(typeof(ApiResponseDto<AutenticacionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRenovarDto dto)
        {
            var result = await _autenticacionService.RefreshToken(dto);
            return HandleResult(result);
        }

        /// <summary>
        /// Cierra sesión (logout).
        /// </summary>
        /// <remarks>
        /// Revoca el refresh token enviado:
        /// - Marca <b>IsUsed=true</b>
        /// - Guarda <b>RevokedAt</b>
        /// <br/><br/>
        /// Nota: No invalida el AccessToken inmediatamente (por diseño de JWT).
        /// La sesión queda cerrada porque el refresh token deja de ser válido.
        /// </remarks>
        /// <param name="dto">Refresh token a revocar.</param>
        /// <response code="200">Logout exitoso. Token revocado.</response>
        /// <response code="400">Token inválido (no existe, expirado, revocado, usado).</response>
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRenovarDto dto)
        {
            var result = await _autenticacionService.Logout(dto);
            return HandleResult(result);
        }
    }
}
