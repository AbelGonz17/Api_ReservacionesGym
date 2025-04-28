using ApiReservacionesGym.DTOs.Clases;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Clase;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Clases")]
    public class ClaseController : ControllerBase
    {
        private readonly IClaseService claseService;

        public ClaseController(IClaseService claseService)
        {
            this.claseService = claseService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClasesDTO>>> Get()
        {
            var resultado = await claseService.ObtenerTodasLasClases();
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return NotFound(resultado);
            }
        }

        [HttpGet("{id:int}", Name = "obtenerClase")]
        public async Task<ActionResult<ClasesDTO>> GetId(int id)
        {
            var resultado = await claseService.ObtenerClasePorId(id);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return NotFound(resultado);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClasesDTO>> Post(CreacionClaseDTO creacionClaseDTO)
        {
            var resultado = await claseService.CrearClase(creacionClaseDTO);
            if(resultado.Exitoso)
            {
                return CreatedAtAction(nameof(GetId), new { id = resultado.Datos.Id }, resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<ClasePatchDTO> patchDocument)
        {
            var resultado = await claseService.EditarClasePatch(id, patchDocument);
            if (resultado.Exitoso)
            {
                return NoContent();
            }
            else
            {
                return NotFound(resultado);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var resultado = await claseService.Delete(id);
            if (resultado.Exitoso)
            {
                return NoContent();
            }
            else
            {
                return NotFound(resultado);
            }
        }
    }
}
