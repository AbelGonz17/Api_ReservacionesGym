using ApiReservacionesGym.DTOs;
using ApiReservacionesGym.DTOs.Reserva;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Reserva;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Reservas")]
    public class ReservaController:ControllerBase
    {
        private readonly IReservaService reservaService;

        public ReservaController(IReservaService reservaService)
        {
            this.reservaService = reservaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReservaDTO>>> GetReservas()
        {
            var resultado = await reservaService.ObtenerReservas();
            if (resultado == null)
            {
                return NotFound(resultado);
            }
            else
            {
                return Ok(resultado);
            }
        }

        [HttpGet("{id:int}", Name = "obtenerReserva")]
        public async Task<ActionResult> GetId(int id)
        {
            var reserva = await reservaService.GetReservaById(id);

            if (!reserva.Exitoso)
            {
                return NotFound(reserva);
            }

            return Ok(reserva);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreacionReservaDTO creacionReservaDTO)
        {
            var resultado = await reservaService.CrearReserva(creacionReservaDTO);

            if (resultado.Exitoso)
            {
                return CreatedAtAction(nameof(GetId), new { id = resultado.Datos.Id }, resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var resultado = await reservaService.Delete(id);

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
