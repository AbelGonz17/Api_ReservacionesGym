using ApiReservacionesGym.Respositorio;

namespace ApiReservacionesGym.Entidades
{
    public class Reserva: IEntidadBase<int>
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public int ClaseId { get; set; }
        public EstadoReserva Estado { get; set; }
        public Cliente Cliente { get; set; }
        public Clase Clase { get; set; }

    }

    public enum EstadoReserva
    {
        Activa =1,
        Cancelada = 2
    }
}
