using ApiReservacionesGym.DTOs.MembresiaDTO;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Membresias;
using AutoMapper;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Membresias")]
    public class MembresiaController : ControllerBase
    {
        private readonly IMembresiaService membresiaService;

        public MembresiaController(IMembresiaService membresiaService)
        {
            this.membresiaService = membresiaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MembresiaDTO>>> Get()
        {
            var resultado = await membresiaService.ObtenerTodasLasMembresias();
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpGet("{id:int}", Name = "ObtenerMembresia")]
        public async Task<ActionResult<MembresiaDTO>> Get(int id)
        {
            var resultado = await membresiaService.ObtenerMembresiaPorId(id);
            if (!resultado.Exitoso)
            {
                return NotFound(resultado);
            }
            return Ok(resultado);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreacionMembresiaDTO creacionMembresiaDTO)
        {
            var resultado = await membresiaService.CrearMembresia(creacionMembresiaDTO);
            if (resultado.Exitoso)
            {
                return CreatedAtAction(nameof(Get), new { id = resultado.Datos.Id }, resultado);
            }
            else
            {
                return BadRequest(resultado);

            }
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<MembresiaPatchDTO> jsonPatchDocument)
        {
            var resultado = await membresiaService.ActualizarMembresia(id, jsonPatchDocument);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return NotFound(resultado);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var resultado = await membresiaService.Delete(id);
            if (resultado.Exitoso)
            {
                return NoContent();
            }
            else
            {
                return NotFound(resultado);
            }
        }

        [HttpPost("ExpirarMembresia/{usuarioId}")]
        public async Task<ActionResult> ExpirarMembresia(string usuarioId)
        {
            var resultado = await membresiaService.ExpirarSuscripcion(usuarioId);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return NotFound(resultado);
            }
        }
    }
    
}

