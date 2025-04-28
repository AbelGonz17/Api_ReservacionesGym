namespace ApiReservacionesGym.DTOs.AuthDTO
{
    public class RespuestaAutenticacionDTO
    {
        public required string Token { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}
