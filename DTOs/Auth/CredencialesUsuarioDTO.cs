using ApiReservacionesGym.Entidades;
using System.ComponentModel.DataAnnotations;

namespace ApiReservacionesGym.DTOs.AuthDTO
{
    public class CredencialesUsuarioDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        public string TipoMembresia { get; set; }



    }
}
