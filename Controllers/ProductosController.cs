using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.DTOs.ProductoDtoCarpeta;
using Mini_E_Commerce_API.Models.Enums;
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

        [Authorize]
        [HttpGet("obtener/{productoId}")]
        public async Task<IActionResult> ObtenerProductoPorId(int productoId)
        {
            if(productoId <= 0)
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su productoId debe de ser un numero"
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

            var producto = await _productoService.ObtenerProductoPorIdAsync(usuarioId, productoId);

            if(!producto.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = producto.Error
                });
            }

            return Ok(new
            {
                success = true,
                valor = producto.Value
            });
        }

        [Authorize]
        [HttpGet("obtener")]
        public async Task<IActionResult> ObtenerProductos()
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

            var productos = await _productoService.ObtenerProductosAsync(usuarioId);

            if (!productos.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = productos.Error
                });
            }

            return Ok(new
            {
                success = true,
                valor = productos.Value
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("incrementar-stock/{productoId}")]
        public async Task<IActionResult> IncrementarStockProducto([FromBody] StockDto stock, int productoId)
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

            var resultadoIncrementar = await _productoService.OpearacionStockProductoAsync(usuarioId, productoId,TipoDeMovimiento.Incrementar,stock.Cantidad);

            if (!resultadoIncrementar.IsSuccess) {
                return BadRequest(new
                {
                    success = false,
                    error = resultadoIncrementar.Error
                });
            }

            return NoContent();
        }


        [Authorize(Roles = "Admin")]
        [HttpPatch("disminuir-stock/{productoId}")]
        public async Task<IActionResult> DisminuirStockProducto([FromBody] StockDto stock, int productoId)
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

            var resultadoIncrementar = await _productoService.OpearacionStockProductoAsync(usuarioId, productoId, TipoDeMovimiento.Disminuir, stock.Cantidad);

            if (!resultadoIncrementar.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = resultadoIncrementar.Error
                });
            }

            return NoContent();
        }
    }
}
