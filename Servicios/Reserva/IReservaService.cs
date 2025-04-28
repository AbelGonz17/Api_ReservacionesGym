using ApiReservacionesGym.DTOs.Reserva;
using ApiReservacionesGym.Servicios.Metodos;

namespace ApiReservacionesGym.Servicios.Reserva
{
    public interface IReservaService
    {
        Task<ResultadoServicio<ReservaDTO>> CrearReserva(CreacionReservaDTO creacionReservaDTO);
        Task<ResultadoServicio<string>> Delete(int id);
        Task<ResultadoServicio<DetalleReservaDTO>> GetReservaById(int id);
        Task<ResultadoServicio<List<DetalleReservaDTO>>> ObtenerReservas();
    }
}