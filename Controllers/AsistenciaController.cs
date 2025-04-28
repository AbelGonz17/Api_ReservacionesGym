using ApiReservacionesGym.DTOs.Asistencia;
using ApiReservacionesGym.Servicios.Asistencias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Asistencias")]
    [Authorize]
    public class AsistenciaController : ControllerBase
    {
        private readonly IAsistenciaService asistenciaService;

        public AsistenciaController(IAsistenciaService asistenciaService)
        {
            this.asistenciaService = asistenciaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<DetalleAsistenciaDTO>>> Get()
        {
            var resultado = await asistenciaService.ObtenerTodasLasAsistencias();
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpGet("{id:int}", Name ="ObtenerAsistencia")]
        public async Task<ActionResult<DetalleAsistenciaDTO>> GetByID(int id)
        {
            var resultado = await asistenciaService.ObtenerAsistencia(id);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreacionAsistenciaDTO creacionAsistenciaDTO)
        {
            var resultado = await asistenciaService.CrearAsistencia(creacionAsistenciaDTO);
            if (resultado.Exitoso)
            {
                return CreatedAtAction(nameof(GetByID), new { id = resultado.Datos.Id }, resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }
    }
}
