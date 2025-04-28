using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Suscripcion
{
    public class SuscripcionMembresiaDTO
    {
        public TipoMembresia Tipo { get; set; }
        public decimal Precio { get; set; }
        public int Duracion { get; set; }
    }
}
