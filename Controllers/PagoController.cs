using ApiReservacionesGym.DTOs.Pago;
using ApiReservacionesGym.Servicios.Pagos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Pagos")]
    public class PagoController: ControllerBase
    {
        private readonly IPagoService pagoService;

        public PagoController(IPagoService pagoService)
        {
            this.pagoService = pagoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PagoDTO>>> GetPagos()
        {
            var resultado = await pagoService.ObtenerPagos();
            if (resultado == null)
            {
                return NotFound(resultado);
            }
            else
            {
                return Ok(resultado);
            }
        }

        [HttpPost]
        public async Task<ActionResult> RealizarPago(CreacionPagoDTO creacionPagoDTO)
        {
            var resultado = await pagoService.RealizarPago(creacionPagoDTO);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }
    }
}
