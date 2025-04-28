using ApiReservacionesGym.Respositorio;

namespace ApiReservacionesGym.Entidades
{
    public class Suscripcion:IEntidadBase<int>
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int MembresiaId { get; set; }
        public Membresia Membresia { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin {  get; set; }
        public EstadoSuscripcion EstadoSuscripcion { get; set; }
    }

    public enum EstadoSuscripcion
    {
        Activo = 1,
        Inactivo = 2,
        Suspendido = 3,
        Cancelado = 4
    }
}
