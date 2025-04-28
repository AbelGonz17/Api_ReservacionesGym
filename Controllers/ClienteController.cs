
using ApiReservacionesGym.DTOs.Clientes;
using ApiReservacionesGym.Servicios.Clientes;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService clienteService;

        public ClienteController(IClienteService clienteService)
        {
            this.clienteService = clienteService;
        }

        [HttpGet("ObtenerClientes")]
        public async Task<ActionResult<List<ClienteDTO>>> Get()
        {
            var resultado = await clienteService.ObtenerClientes();
            if (resultado == null)
            {
                return NotFound();
            }
            return Ok(resultado);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ClienteDTO>> GetById(Guid id)
        {
            var resultado = await clienteService.ObtenerCliente(id);
            if (resultado == null)
            {
                return NotFound();
            }
            return Ok(resultado);

        }
    }
}
