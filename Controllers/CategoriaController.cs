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
    public class CategoriaController : BaseApiController
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService) { 
            _categoriaService = categoriaService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CrearCategoria([FromBody] CategoriaCrearDto categoriaCrearDto)
        {
           if(!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var categoria = await _categoriaService.CrearCategoriaAsync(categoriaCrearDto, usuarioId);

            return HandleResult(categoria);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{categoriaId}")]
        public async Task<IActionResult> ActualizarCategoria(
            int categoriaId,
            [FromBody] CategoriaCrearDto dto)
        {

            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var result = await _categoriaService
                .ActualizarCategoriaAsync(dto, categoriaId, usuarioId);

            return HandleResult(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoriaId}")]
        public async Task<IActionResult> DesactivarCategoria(int categoriaId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var result = await _categoriaService
                .DesactivarCategoriaAsync(usuarioId, categoriaId);

            return HandleResult(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerCategorias()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var result = await _categoriaService.ObtenerCategoriasAsync(usuarioId);

            return HandleResult(result);
        }
    }
}
