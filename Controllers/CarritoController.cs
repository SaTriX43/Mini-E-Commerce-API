using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Services.CarritoServiceCarpeta;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService) { 
            _carritoService = carritoService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerCarritoUsuario()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su usuarioId debe de ser un numero"
                });
            }

            var carrito = await _carritoService.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            if(!carrito.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = carrito.Error
                });
            }

            return Ok(new
            {
                success = true,
                value = carrito.Value
            });
        }
    }
}
