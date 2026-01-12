using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("registro")]
        public async Task<IActionResult> Registrar(RegistroRequestDto dto)
        {
           
            var result = await _autenticacionService.RegistrarAsync(dto);

            return HandleResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto dto) 
        {
            var result = await _autenticacionService.LoginAsync(dto);

            return HandleResult(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRenovarDto dto)
        {

            var result = await _autenticacionService.RefreshToken(dto);

            return HandleResult(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout(RefreshTokenRenovarDto dto)
        {
            var result = await _autenticacionService.Logout(dto);

            return HandleResult(result);
        }
    }

}
