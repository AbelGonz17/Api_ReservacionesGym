using ApiReservacionesGym.DTOs.Suscripcion;
using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.SuscripcionDTO
{
    public class SuscripcionDTOs
    {
        public int Id { get; set; }
        public SuscripcionMembresiaDTO Membresia { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public EstadoSuscripcion EstadoSuscripcion { get; set; }
    }
}
