using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Services.OrdenServiceCarpeta;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {
        private readonly IOrdenService _ordenService;

        public OrdenesController(IOrdenService ordenService) {
            _ordenService = ordenService;
        }

        [Authorize]
        [HttpPost("crear")]
        public async Task<IActionResult> CrearOrden()
        {
            var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!int.TryParse(usuarioIdClaim, out int usuarioId))
            {
                return BadRequest(new
                {
                    success = false,
                    error = "Su usuario id debe de ser un numero"
                });
            }

            var orden = await _ordenService.CrearOrdenAsync(usuarioId);

            if(!orden.IsSuccess)
            {
                return BadRequest(new
                {
                    success = false,
                    error = orden.Error
                });
            }

            return Ok(new
            {
                success = true,
                value = orden.Value
            });
        }
    }
}
