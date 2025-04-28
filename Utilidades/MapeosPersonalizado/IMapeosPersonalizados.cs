using ApiReservacionesGym.DTOs.Reserva;
using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.Utilidades.MapeosPersonalizado
{
    public interface IMapeosPersonalizados
    {
        Reserva MapReserva(CreacionReservaDTO creacionReservaDTO, Clase clases, Guid clienteId);
    }
}