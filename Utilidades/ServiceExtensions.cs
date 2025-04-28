using ApiReservacionesGym.Respositorio;
using ApiReservacionesGym.Servicios.Asistencias;
using ApiReservacionesGym.Servicios.Clase;
using ApiReservacionesGym.Servicios.Clientes;
using ApiReservacionesGym.Servicios.Membresias;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.Servicios.Pagos;
using ApiReservacionesGym.Servicios.Reserva;
using ApiReservacionesGym.Servicios.Suscripciones;
using ApiReservacionesGym.Servicios.Users;
using ApiReservacionesGym.UnitOfWork;
using ApiReservacionesGym.Utilidades.MapeosPersonalizado;

namespace ApiReservacionesGym.Utilidades
{
    public static class ServiceExtensions
    {
        public static void AddApplicationServices(this  IServiceCollection services)
        {   
            services.AddScoped<IReservaService, ReservaService>();
            services.AddScoped<IClaseService, ClaseService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IUsuarioActualHelper, UsuarioActualHelper>();
            services.AddScoped<IMembresiaService, MembresiaService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAsistenciaService, AsistenciaService>();
            services.AddScoped<IPagoService, PagoService>();
            services.AddScoped<ISuscripcionesServicio, SuscripcionesServicio>();
            services.AddScoped<IMapeosPersonalizados, MapeosPersonalizados>();

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        }
    }
}
