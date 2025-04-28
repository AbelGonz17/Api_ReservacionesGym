using System.ComponentModel.DataAnnotations;

namespace ApiReservacionesGym.DTOs.AuthDTO
{
    public class CredencialesLoginDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
