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
    public class CarritoController : BaseApiController
    {
        private readonly ICarritoService _carritoService;

        public CarritoController(ICarritoService carritoService) { 
            _carritoService = carritoService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerCarritoUsuario()
        {
           if(!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var carrito = await _carritoService.ObtenerCarritoPorUsuarioIdAsync(usuarioId);

            return HandleResult(carrito);
        }

        [Authorize]
        [HttpPost("agregar-item")]
        public async Task<IActionResult> AgregarCarritoItem([FromBody] CarritoItemAgregarDto itemAgregarDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultado = await _carritoService.AgregarCarritoItemAsync(itemAgregarDto, usuarioId);

            return HandleResult(resultado);
        }

        [Authorize]
        [HttpPatch("actualizar-cantidad-item")]
        public async Task<IActionResult> ActualizarCantidadCarritoItem([FromBody] CarritoItemAgregarDto itemAgregarDto)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultado = await _carritoService.ActualizarCantidadCarritoItemAsync(itemAgregarDto, usuarioId);

            return HandleResult(resultado);
        }

        [Authorize]
        [HttpDelete("eliminar-item/{carritoItemId}")]
        public async Task<IActionResult> ActualizarCantidadCarritoItem(int carritoItemId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultado = await _carritoService.EliminarCarritoItemAsync(carritoItemId, usuarioId);

            return HandleResult(resultado);
        }

        [Authorize]
        [HttpDelete("vaciar")]
        public async Task<IActionResult> VaciarCarrito()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultado = await _carritoService.VaciarCarritoAsync(usuarioId);

            return HandleResult(resultado);
        }
    }
}
