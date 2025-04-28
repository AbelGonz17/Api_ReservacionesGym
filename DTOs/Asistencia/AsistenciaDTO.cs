using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Asistencia
{
    public class AsistenciaDTO
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public int ClaseId { get; set; }
        public Cliente Cliente { get; set; }
        public Clase Clase { get; set; }
    }
}
