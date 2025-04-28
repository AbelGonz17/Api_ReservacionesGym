
using ApiReservacionesGym.DTOs.Clientes;
using ApiReservacionesGym.Servicios.Metodos;

namespace ApiReservacionesGym.Servicios.Clientes
{
    public interface IClienteService
    {
        Task<ResultadoServicio<ClienteDTO>> ObtenerCliente(Guid id);
        Task<ResultadoServicio<List<ClienteDTO>>> ObtenerClientes();
    }
}