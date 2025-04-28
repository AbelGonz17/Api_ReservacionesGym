namespace ApiReservacionesGym.DTOs.Reserva
{
    public class ClaseReservaDTO
    {
        public string Nombre { get; set; }
        public string Intructor { get; set; }
        public TimeSpan Hora { get; set; }
        public List<string> ClaseDias { get; set; }
    }
}
