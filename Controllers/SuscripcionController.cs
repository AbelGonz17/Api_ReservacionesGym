using ApiReservacionesGym.DTOs.Suscripcion;
using ApiReservacionesGym.Servicios.Suscripciones;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Suscripciones")]
    public class SuscripcionController:ControllerBase
    {
        private readonly ISuscripcionesServicio suscripcionesServicio;

        public SuscripcionController(ISuscripcionesServicio suscripcionesServicio)
        {
            this.suscripcionesServicio = suscripcionesServicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuscripcionDetalleDTO>>> GetSuscripciones()
        {
            var resultado = await suscripcionesServicio.ObtenerSuscripciones();
            if (resultado == null)
            {
                return NotFound(resultado);
            }
            else
            {
                return Ok(resultado);
            }
        }

    }
}
