using ApiReservacionesGym.Respositorio;

namespace ApiReservacionesGym.Entidades
{
    public class Asistencia:IEntidadBase<int>
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public int ClaseId  { get; set; }
        public DateTime Fecha { get; set; } = DateTime.UtcNow;
        public Cliente Cliente  { get; set; }
        public Clase Clase { get; set; }

    }
}
