using ApiReservacionesGym.Respositorio;

namespace ApiReservacionesGym.Entidades
{
    public class Membresia : IEntidadBase<int>
    {
        public int Id { get; set; }
        public TipoMembresia Tipo { get; set; }
        public decimal Precio { get; set; }
        public int Duracion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public EstadoMembresia Estado { get; set; }
    }

    public enum EstadoMembresia
    {
       Activo = 1,
       Inactivo = 2,
       Suspendido = 3,
       Cancelado = 4
    }

    public enum TipoMembresia
    {
        Basico = 1,
        Premium = 2,
        VIP = 3
    }
}
