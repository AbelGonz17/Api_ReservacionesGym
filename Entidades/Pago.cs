using ApiReservacionesGym.Respositorio;

namespace ApiReservacionesGym.Entidades
{
    public class Pago:IEntidadBase<int>
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public MetodosDePago MetodoDePago { get; set; }
        public Cliente Cliente { get; set; }
    }

    public enum MetodosDePago
    {
        efectivo = 1,
        tarjeta = 2

    }
}
