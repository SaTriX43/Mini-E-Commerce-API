using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.DTOs.CarritoDtoCarpeta;
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

        [Authorize]
        [HttpPost("agregar-item")]
        public async Task<IActionResult> AgregarCarritoItem([FromBody] CarritoItemAgregarDto itemAgregarDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ModelState
                });
            }

            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su usuarioId debe de ser un numero"
                });
            }

            var resultado = await _carritoService.AgregarCarritoItemAsync(itemAgregarDto, usuarioId);

            if (!resultado.IsSuccess) {
                return BadRequest(new
                {
                    success = false,
                    error = resultado.Error
                });
            }

            return NoContent();
        }

        [Authorize]
        [HttpPatch("actualizar-cantidad-item")]
        public async Task<IActionResult> ActualizarCantidadCarritoItem([FromBody] CarritoItemAgregarDto itemAgregarDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    error = ModelState
                });
            }

            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su usuarioId debe de ser un numero"
                });
            }

            var resultado = await _carritoService.ActualizarCantidadCarritoItemAsync(itemAgregarDto, usuarioId);

            if (!resultado.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = resultado.Error
                });
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("eliminar-item/{carritoItemId}")]
        public async Task<IActionResult> ActualizarCantidadCarritoItem(int carritoItemId)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su usuarioId debe de ser un numero"
                });
            }

            var resultado = await _carritoService.EliminarCarritoItemAsync(carritoItemId, usuarioId);

            if (!resultado.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = resultado.Error
                });
            }

            return NoContent();
        }

        [Authorize]
        [HttpDelete("vaciar")]
        public async Task<IActionResult> VaciarCarrito()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su usuarioId debe de ser un numero"
                });
            }

            var resultado = await _carritoService.VaciarCarritoAsync(usuarioId);

            if (!resultado.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = resultado.Error
                });
            }

            return NoContent();
        }
    }
}
