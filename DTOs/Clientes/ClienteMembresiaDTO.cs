using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Clientes
{
    public class ClienteMembresiaDTO
    {
        public int Id { get; set; }
        public TipoMembresia Tipo { get; set; }
        public decimal Precio { get; set; }
        public int Duracion { get; set; }
    }
}
