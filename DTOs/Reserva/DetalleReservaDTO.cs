using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Reserva
{
    public class DetalleReservaDTO
    {
        public int Id { get; set; }
        public EstadoReserva Estado { get; set; }
        public ClienteReservaDTO Cliente { get; set; }
        public ClaseReservaDTO Clase { get; set; }
    }
}
