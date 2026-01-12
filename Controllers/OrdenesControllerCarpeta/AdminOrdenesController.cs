using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Services.OrdenServiceCarpeta;

namespace Mini_E_Commerce_API.Controllers.OrdenesControllerCarpeta
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminOrdenesController : BaseApiController
    {
        private readonly IOrdenService _ordenService;

        public AdminOrdenesController(IOrdenService ordenService)
        {
           _ordenService = ordenService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet()]
        public async Task<IActionResult> ObtenerOrdenesUsuarios()
        {
            var ordenes = await _ordenService.ObtenerTodasLasOrdenesAsync();

            return HandleResult(ordenes);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ordenes-usuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerOrdenesUsuarioPorId(int usuarioId)
        {
            var ordenes = await _ordenService.ObtenerOrdenesPorUsuarioAdminAsync(usuarioId);

            return HandleResult(ordenes);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("orden-detalles/{ordenId}")]
        public async Task<IActionResult> ObtenerOrdenDetallesPorId(int ordenId)
        {
            var ordenes = await _ordenService.ObtenerOrdenDetallesUsuarioAdminAsync(ordenId);

            return HandleResult(ordenes);
        }
    }
}
