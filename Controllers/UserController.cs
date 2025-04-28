using ApiReservacionesGym.DTOs.AuthDTO;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiReservacionesGym.Controllers
{
    [ApiController]
    [Route("api/Users")]
    public class UserController:ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("Registro")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Registrar(CredencialesUsuarioDTO credencialesUsuarioDTO)
        {
            var resultado = await userService.RegistrarAsync(credencialesUsuarioDTO);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<RespuestaAutenticacionDTO>> Login(CredencialesLoginDTO credencialesUsuario)
        {
            var resultado = await userService.LoginAsync(credencialesUsuario);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return RetornarLoginIncorrecto();
            }

        }

        [HttpPost("Actualizar-Rol")]
        public async Task<ActionResult> ActualizarRol(ActualizarRolDTO actualizarRolDTO)
        {
            var resultado = await userService.PromeverInstructor(actualizarRolDTO);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        [HttpPost("remover-Instructor")]
        public async Task<ActionResult> RemoverAdmin(EditarClaimDTO editarClaimDTO)
        {
            var resultado = await userService.RemoverAdmin(editarClaimDTO);
            if (resultado.Exitoso)
            {
                return Ok(resultado);
            }
            else
            {
                return BadRequest(resultado);
            }
        }

        private ActionResult RetornarLoginIncorrecto()
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrecta");
            return ValidationProblem();
        }

    }
}
