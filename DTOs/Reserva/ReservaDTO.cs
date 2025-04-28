using ApiReservacionesGym.Entidades;

    public class ReservaDTO
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public int ClaseId { get; set; }
        public EstadoReserva Estado { get; set; }
        public Cliente Cliente { get; set; }
        public Clase Clase { get; set; }
    }

