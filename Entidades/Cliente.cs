using ApiReservacionesGym.Respositorio;

namespace ApiReservacionesGym.Entidades
{
    public class Cliente:IEntidadBase<Guid>
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }
        public ICollection<Suscripcion> Suscripciones { get; set; }
    }
}
