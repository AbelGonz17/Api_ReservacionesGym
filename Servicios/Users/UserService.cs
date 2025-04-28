using ApiReservacionesGym.DTOs.AuthDTO;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.Servicios.Suscripciones;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiReservacionesGym.Servicios.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly ISuscripcionesServicio suscripcionesServicio;
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogger<UserService> logger;

        public UserService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            IMapper mapper,ISuscripcionesServicio suscripcionesServicio,
            IUnitOfWork unitOfWork,ILogger<UserService> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.mapper = mapper;
            this.suscripcionesServicio = suscripcionesServicio;
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public async Task<ResultadoServicio<RespuestaAutenticacionDTO>> RegistrarAsync(CredencialesUsuarioDTO dto)
        {
            var clienteExistente = await unitOfWork.Cliente
                .GetByConditionAsync(c => c.Email == dto.Email && c.Suscripciones
                .Any(s => s.EstadoSuscripcion == EstadoSuscripcion.Activo));

            if (clienteExistente != null)
            {
                return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo("Este usuario ya está suscrito a una membresía activa.");
            }

            //Convertimos la cadena en un enum
            if (string.IsNullOrWhiteSpace(dto.TipoMembresia) ||
                !Enum.TryParse<TipoMembresia>(dto.TipoMembresia, true, out var tipoMembresia))
            {
                var tiposDisponibles = string.Join(", ", Enum.GetNames(typeof(TipoMembresia)));
                return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo($"Tipo de membresía inválido. Las opciones disponibles son: {tiposDisponibles}");
            }

            //aqui y ya obtenemos el Membresia iD
            var membresia = await unitOfWork.Membresia
                .GetByConditionAsync(m => m.Tipo == tipoMembresia);

            if (membresia == null)
            {
                return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo("No se encontró la membresía.");
            }

            var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };
            var resultado = await userManager.CreateAsync(user, dto.Password);

            if (!resultado.Succeeded)
            {
                return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo("Error al crear el usuario.");
            }

            try
            {
                var cliente = new Cliente
                {
                    Id = new Guid(user.Id),
                    Email = dto.Email,
                    Nombre = dto.Name,
                    FechaRegistro = DateTime.UtcNow
                };

                
                await unitOfWork.Cliente.AddAsync(cliente);              
                await unitOfWork.SaveChangesAsync();

                await suscripcionesServicio.AsignarSuscripcionInicial(cliente.Id, membresia.Id);

                await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Cliente"));

                return await ConstruirTokenAsync(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al registrar usuario: {Mensaje}", ex.InnerException?.Message ?? ex.Message);
                return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo("Error inesperado al registrar usuario.");
            }
        }

        public async Task<ResultadoServicio<RespuestaAutenticacionDTO>> LoginAsync(CredencialesLoginDTO dto)
        {
            var usuario = await userManager.FindByEmailAsync(dto.Email);
            if (usuario == null)
            {
                return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo("Usuario no encontrado.");
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, dto.Password, false);
            if (!resultado.Succeeded)
            {
                return RetornarLoginIncorrecto();
            }

            var usuarioDTO = new CredencialesUsuarioDTO
            {
                Email = dto.Email,
                Password = dto.Password,
                Name = usuario.UserName
            };

            return await ConstruirTokenAsync(usuarioDTO);
        }

        public async Task<ResultadoServicio<string>> PromeverInstructor(ActualizarRolDTO actualizarRolDTO)
        {
            var usuario = await userManager.FindByEmailAsync(actualizarRolDTO.Email);
            if (usuario == null)
            {
                return ResultadoServicio<string>.Fallo("Usuario no encontrado.");
            }

            var claims = await userManager.GetClaimsAsync(usuario);

            var rolInstructor = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == "Instructor");
            if (rolInstructor != null)
            {
                return ResultadoServicio<string>.Fallo("El usuario ya tiene el rol de instructor.");
            }

            var rolesActuales = claims.Where(c => c.Type == ClaimTypes.Role).ToList();

            foreach (var rol in rolesActuales)
            {
                await userManager.RemoveClaimAsync(usuario, rol);
            }

            var rolesPermitidos = new[] { "Cliente", "Instructor" };
            if (!rolesPermitidos.Contains(actualizarRolDTO.NuevoRol))
            {
                return ResultadoServicio<string>.Fallo($"El rol {actualizarRolDTO.NuevoRol} no es válido. Los roles permitidos son: {string.Join(", ", rolesPermitidos)}");
            }

            var newClaim = new Claim(ClaimTypes.Role, actualizarRolDTO.NuevoRol);
            var resultado = await userManager.AddClaimAsync(usuario, newClaim);

            if (resultado == null)
            {
                return ResultadoServicio<string>.Fallo("Error al agregar el rol.");
            }

            return ResultadoServicio<string>.Ok("Rol de instructor agregado.");
        }

        public async Task<ResultadoServicio<string>> RemoverAdmin(EditarClaimDTO editarClaimDTO)
        {
            var usuario = await userManager.FindByEmailAsync(editarClaimDTO.Email);
            if (usuario == null)
            {
                return ResultadoServicio<string>.Fallo("Usuario no encontrado.");
            }
            var claims = await userManager.GetClaimsAsync(usuario);
            var claimToRemove = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role && c.Value == "Instructor");
            if (claimToRemove != null)
            {
                await userManager.RemoveClaimAsync(usuario, claimToRemove);
                return ResultadoServicio<string>.Ok("Rol de Instructor eliminado.");
            }
            return ResultadoServicio<string>.Fallo("El usuario no tiene el rol de instructor.");
        }

        private ResultadoServicio<RespuestaAutenticacionDTO> RetornarLoginIncorrecto()
        {
            return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo("Usuario o contraseña incorrectos.");
        }

        private async Task<ResultadoServicio<RespuestaAutenticacionDTO>> ConstruirTokenAsync(CredencialesUsuarioDTO dto)
        {
            var cliente = await unitOfWork.Cliente
                .GetByConditionAsync(c => c.Email == dto.Email);

            if (cliente == null)
            {
                return ResultadoServicio<RespuestaAutenticacionDTO>.Fallo("Cliente no encontrado.");
            }

            var claims = new List<Claim>
            {
                new Claim("Email", dto.Email),
                new Claim("UsuarioId", cliente.Id.ToString())
            };

            var user = await userManager.FindByEmailAsync(dto.Email);
            var claimsDB = await userManager.GetClaimsAsync(user);
            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new ResultadoServicio<RespuestaAutenticacionDTO>
            {
                Exitoso = true,
                Datos = new RespuestaAutenticacionDTO
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    FechaExpiracion = expiration
                }
            };

        }
    }
}
