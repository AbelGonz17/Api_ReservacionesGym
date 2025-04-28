using ApiReservacionesGym.DTOs.AuthDTO;
using ApiReservacionesGym.Servicios.Metodos;

namespace ApiReservacionesGym.Servicios.Users
{
    public interface IUserService
    {
        Task<ResultadoServicio<RespuestaAutenticacionDTO>> LoginAsync(CredencialesLoginDTO dto);
        Task<ResultadoServicio<string>> PromeverInstructor(ActualizarRolDTO actualizarRolDTO);
        Task<ResultadoServicio<RespuestaAutenticacionDTO>> RegistrarAsync(CredencialesUsuarioDTO dto);
        Task<ResultadoServicio<string>> RemoverAdmin(EditarClaimDTO editarClaimDTO);
    }
}