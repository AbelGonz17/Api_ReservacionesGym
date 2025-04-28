using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Suscripcion
{
    public class SuscripcionDetalleDTO
    {
        public int Id { get; set; }
        public SuscripcionClienteDTO Cliente { get; set; }
        public SuscripcionMembresiaDTO Membresia { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public EstadoSuscripcion EstadoSuscripcion { get; set; }

    }
}
