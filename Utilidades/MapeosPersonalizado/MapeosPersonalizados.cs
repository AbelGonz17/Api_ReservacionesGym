using ApiReservacionesGym.DTOs.Reserva;
using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.Utilidades.MapeosPersonalizado
{
    public class MapeosPersonalizados : IMapeosPersonalizados
    {
        public Reserva MapReserva(CreacionReservaDTO creacionReservaDTO, Clase clases, Guid clienteId)
        {

            var reserva = new Reserva
            {
                ClienteId = clienteId,
                ClaseId = creacionReservaDTO.ClaseId,
                Estado = EstadoReserva.Activa,
                Clase = clases
            };

            return reserva;
        }
    }
}
