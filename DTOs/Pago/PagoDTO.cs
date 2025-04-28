using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Pago
{
    public class PagoDTO
    {
        public int Id { get; set; }
        public Guid ClienteId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public MetodosDePago MetodoDePago { get; set; }
    }
}
