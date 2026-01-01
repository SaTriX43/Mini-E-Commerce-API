using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.DTOs.CategoriaDtoCarpeta;
using Mini_E_Commerce_API.Services.CategoriaServiceCarpeta;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService) { 
            _categoriaService = categoriaService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaCrearDto categoriaCrearDto)
        {
            if (!ModelState.IsValid) { 
                return BadRequest(new
                {
                    success = false,
                    error = ModelState
                });
            }

            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId)) {
                return BadRequest(new
                {
                    success = false,
                    error = "El id de su usuario debe ser un numero"
                });
            }

            var categoria = await _categoriaService.CrearCategoriaAsync(categoriaCrearDto, usuarioId);

            if(!categoria.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = categoria.Error
                });
            }

            return Ok(categoria.Value);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("actualizar/{categoriaId}")]
        public async Task<IActionResult> ActualizarCategoria(
            int categoriaId,
            [FromBody] CategoriaCrearDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "El usuarioId no es válido"
                });
            }

            var result = await _categoriaService
                .ActualizarCategoriaAsync(dto, categoriaId, usuarioId);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = result.Error
                });
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("desactivar/{categoriaId}")]
        public async Task<IActionResult> DesactivarCategoria(int categoriaId)
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "El usuarioId no es válido"
                });
            }

            var result = await _categoriaService
                .DesactivarCategoriaAsync(usuarioId, categoriaId);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = result.Error
                });
            }

            return NoContent();
        }
    }
}
