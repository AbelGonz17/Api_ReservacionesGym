namespace ApiReservacionesGym.Entidades
{
    public class ClaseDia
    {
        public int Id { get; set; }
        public DiaSemana Dia { get; set; }
        public int ClaseId { get; set; }
        public Clase Clase { get; set; }

    }

    public enum DiaSemana
    {
        Lunes = 1,
        Martes = 2,
        Miercoles = 3,
        Jueves = 4,
        Viernes = 5,
        Sabado = 6,
        Domingo = 7
    }
}
