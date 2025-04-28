using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Pago
{
    public class CreacionPagoDTO
    {
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string MetodoDePago { get; set; }
        
    }
}
