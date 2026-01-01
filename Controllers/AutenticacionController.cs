using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.DTOs.AutenticacionDtoCarpeta;
using Mini_E_Commerce_API.Services.AutenticacionServiceCarpeta;

namespace Mini_E_Commerce_API.Controllers
{
    [ApiController]
    [Route("api/autenticacion")]
    public class AutenticacionController : ControllerBase
    {
        private readonly IAutenticacionService _autenticacionService;

        public AutenticacionController(IAutenticacionService autenticacionService)
        {
            _autenticacionService = autenticacionService;
        }

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar(RegistroRequestDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _autenticacionService.RegistrarAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto) 
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _autenticacionService.LoginAsync(dto);

            if (!result.IsSuccess)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRenovarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _autenticacionService.RefreshToken(dto);

            if (!result.IsSuccess)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRenovarDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _autenticacionService.Logout(dto);

            if (!result.IsSuccess)
                return Unauthorized(result.Error);

            return NoContent();
        }
    }

}
