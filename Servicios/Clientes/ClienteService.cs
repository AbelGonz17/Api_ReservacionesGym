using ApiReservacionesGym.DTOs.Clientes;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;

namespace ApiReservacionesGym.Servicios.Clientes
{
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ClienteService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ResultadoServicio<List<ClienteDTO>>> ObtenerClientes()
        {
            var clientes = await unitOfWork.Cliente.GetAllAsync("Suscripciones,Suscripciones.Membresia  ");
            if (clientes == null || !clientes.Any())
            {
                return ResultadoServicio<List<ClienteDTO>>.Fallo("No se encontraron los Clientes");
            }
            var clienteDTO = mapper.Map<List<ClienteDTO>>(clientes);
            return ResultadoServicio<List<ClienteDTO>>.Ok(clienteDTO);
        }

        public async Task<ResultadoServicio<ClienteDTO>> ObtenerCliente(Guid id)
        {
            var cliente = await unitOfWork.Cliente.GetByAsyncId(id,"Membresia");
            if (cliente == null)
            {
                return ResultadoServicio<ClienteDTO>.Fallo($"El cliente con el ID {id} no existe");
            }

            var dto = mapper.Map<ClienteDTO>(cliente);
            return ResultadoServicio<ClienteDTO>.Ok(dto);

        }
    }
}
