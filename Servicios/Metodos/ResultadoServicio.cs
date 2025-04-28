namespace ApiReservacionesGym.Servicios.Metodos
{
    //Con este formato de respuesta 
    //nos da control para manejar errores sin tirar excepciones.
    // Es fácil de consumir en el frontend(podemos verificar .exitoso antes de mostrar algo).
    // nos  mantiene consistencia en toda la API, que es lo más importante en proyectos grandes.
    public class ResultadoServicio<T>
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public T Datos { get; set; }

        public static ResultadoServicio<T> Fallo(string mensaje) => new() { Exitoso = false, Mensaje = mensaje };
        public static ResultadoServicio<T> Ok(T datos) => new() { Exitoso = true, Datos = datos };
    }
}
