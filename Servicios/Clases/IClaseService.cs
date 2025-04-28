using ApiReservacionesGym.DTOs.Clases;
using ApiReservacionesGym.Servicios.Metodos;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiReservacionesGym.Servicios.Clase
{
    public interface IClaseService
    {
        Task<ResultadoServicio<ClasesDTO>> CrearClase(CreacionClaseDTO creacionClaseDTO);
        Task<ResultadoServicio<string>> Delete(int id);
        Task<ResultadoServicio<ClasesDTO>> EditarClasePatch(int id, JsonPatchDocument<ClasePatchDTO> patchDocument);
        Task<ResultadoServicio<ClasesDTO>> ObtenerClasePorId(int id);
        Task<ResultadoServicio<List<ClasesDTO>>> ObtenerTodasLasClases();
    }
}