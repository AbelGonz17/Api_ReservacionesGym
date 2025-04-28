using ApiReservacionesGym.DTOs.Asistencia;
using ApiReservacionesGym.Servicios.Metodos;

namespace ApiReservacionesGym.Servicios.Asistencias
{
    public interface IAsistenciaService
    {
        Task<ResultadoServicio<DetalleAsistenciaDTO>> CrearAsistencia(CreacionAsistenciaDTO creacionAsistenciaDTO);
        Task<ResultadoServicio<DetalleAsistenciaDTO>> ObtenerAsistencia(int id);
        Task<ResultadoServicio<List<DetalleAsistenciaDTO>>> ObtenerTodasLasAsistencias();
    }
}