namespace ApiReservacionesGym.DTOs.Asistencia
{
    public class DetalleAsistenciaDTO
    {
       public int Id { get; set; }
       public DateTime Fecha { get; set; } = DateTime.UtcNow;
       public Guid ClienteId { get; set; }
       public string NombreCliente { get; set; }
       public string EmailCliente { get; set; }
       public int ClaseId { get; set; }
       public string NombreClase { get; set; }
       public string Intructor { get; set; }
        
    }
}
