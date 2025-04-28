namespace ApiReservacionesGym.Servicios.Metodos
{
    public class UsuarioActualHelper : IUsuarioActualHelper
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UsuarioActualHelper(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public Guid ObtenerUsuarioId()
        {
            var usuarioId = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "UsuarioId")?.Value;
            if (usuarioId == null)
            {
                throw new Exception("No se encontró el usuario actual");
            }
            return Guid.Parse(usuarioId);

        }
    }
}

