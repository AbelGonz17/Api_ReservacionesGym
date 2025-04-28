using ApiReservacionesGym.DTOs.MembresiaDTO;
using ApiReservacionesGym.DTOs.Suscripcion;
using ApiReservacionesGym.DTOs.SuscripcionDTO;
using ApiReservacionesGym.Entidades;

namespace ApiReservacionesGym.DTOs.Clientes
{
    public class ClienteDTO
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }
        public ICollection<SuscripcionDTOs> Suscripciones { get; set; }

    }
}
