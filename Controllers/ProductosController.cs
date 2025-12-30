using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta;
using Mini_E_Commerce_API.Services.ProductoServiceCarpeta;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductosController(IProductoService productoService) { 
            _productoService = productoService;
        }

        [Authorize]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProducto([FromBody] ProductoCrearDto productoCrearDto)
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
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if(!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su usuarioId debe de ser un numero"
                });
            }

            var productoCreado = await _productoService.CrearProductoAsync(productoCrearDto,usuarioId,rol);

            if(!productoCreado.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = productoCreado.Error
                });
            }

            return Ok(new
            {
                success = true,
                valor = productoCreado.Value
            });
        }
    }
}
