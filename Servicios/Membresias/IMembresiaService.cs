
using ApiReservacionesGym.DTOs.MembresiaDTO;
using ApiReservacionesGym.Servicios.Metodos;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiReservacionesGym.Servicios.Membresias
{
    public interface IMembresiaService
    {
        Task<ResultadoServicio<MembresiaDTO>> ActualizarMembresia(int id, JsonPatchDocument<MembresiaPatchDTO> patchDocument);
        Task<ResultadoServicio<MembresiaDTO>> CrearMembresia(CreacionMembresiaDTO creacionMembresiaDTO);
        Task<ResultadoServicio<string>> Delete(int id);
        Task<ResultadoServicio<string>> ExpirarSuscripcion(string usuarioId);
        Task<ResultadoServicio<MembresiaDTO>> ObtenerMembresiaPorId(int id);
        Task<ResultadoServicio<List<MembresiaDTO>>> ObtenerTodasLasMembresias();
    }
}