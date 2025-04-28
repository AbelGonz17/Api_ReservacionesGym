using ApiReservacionesGym.Respositorio;

namespace ApiReservacionesGym.Entidades
{
    public class Clase:IEntidadBase<int>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Intructor { get; set; }      
        public TimeSpan Hora { get; set; }
        public int CupoMaximo { get; set; }
        public List<ClaseDia> ClaseDias { get; set; } = new();
    }

}
