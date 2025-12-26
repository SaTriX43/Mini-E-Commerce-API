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
            var result = await _autenticacionService.RegistrarAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            var result = await _autenticacionService.LoginAsync(dto);

            if (!result.IsSuccess)
                return Unauthorized(result.Error);

            return Ok(result.Value);
        }
    }

}
