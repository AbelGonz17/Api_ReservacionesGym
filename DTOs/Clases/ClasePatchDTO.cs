namespace ApiReservacionesGym.DTOs.Clases
{
    public class ClasePatchDTO
    {
        public string Nombre { get; set; }
        public string Intructor { get; set; }
        public TimeSpan Hora { get; set; }
        public int CupoMaximo { get; set; }
        public List<string> ClaseDias { get; set; }
    }
}
