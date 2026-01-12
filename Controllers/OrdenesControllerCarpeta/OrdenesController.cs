using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mini_E_Commerce_API.Services.OrdenServiceCarpeta;
using System.Security.Claims;

namespace Mini_E_Commerce_API.Controllers.OrdenesControllerCarpeta
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdenesController : BaseApiController
    {
        private readonly IOrdenService _ordenService;

        public OrdenesController(IOrdenService ordenService) {
            _ordenService = ordenService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CrearOrden()
        {
            if(!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var orden = await _ordenService.CrearOrdenAsync(usuarioId);

            return HandleResult(orden);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ObtenerOrdenes()
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var ordenes = await _ordenService.ObtenerOrdenesUsuarioAsync(usuarioId);

            return HandleResult(ordenes);
        }

        [Authorize]
        [HttpGet("{ordenId}")]
        public async Task<IActionResult> ObtenerOrdenDetallesPorId(int ordenId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var ordenDetalles = await _ordenService.ObtenerOrdenDetallesUsuarioAsync(ordenId,usuarioId);

            return HandleResult(ordenDetalles);
        }

        [Authorize]
        [HttpPost("cancelar/{ordenId}")]
        public async Task<IActionResult> CancelarOrden(int ordenId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultado = await _ordenService.CancelarOrdenAsync(ordenId, usuarioId);

            return HandleResult(resultado);
        }

        [Authorize]
        [HttpPost("pagar/{ordenId}")]
        public async Task<IActionResult> PagarOrden(int ordenId)
        {
            if (!TryGetUserId(out var usuarioId, out var error))
            {
                return error;
            }

            var resultado = await _ordenService.PagarOrdenAsync(ordenId, usuarioId);

            return HandleResult(resultado);
        }
    }
}
