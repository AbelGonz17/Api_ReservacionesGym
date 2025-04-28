using ApiReservacionesGym.DTOs.Suscripcion;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Migrations;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace ApiReservacionesGym.Servicios.Suscripciones
{
    public class SuscripcionesServicio : ISuscripcionesServicio
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly ILogger<SuscripcionesServicio> logger;

        public SuscripcionesServicio(IUnitOfWork unitOfWork, IMapper mapper,ILogger<SuscripcionesServicio> logger)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.logger = logger;
        }

        public async Task<ResultadoServicio<List<SuscripcionDetalleDTO>>> ObtenerSuscripciones()
        {
            var suscripciones = await unitOfWork.Suscripcion.GetAllAsync("Cliente, Membresia");
            if (suscripciones == null || !suscripciones.Any())
            {
                return ResultadoServicio<List<SuscripcionDetalleDTO>>.Fallo("No hay suscripciones.");
            }


            var dto = mapper.Map<List<SuscripcionDetalleDTO>>(suscripciones);

            return ResultadoServicio<List<SuscripcionDetalleDTO>>.Ok(dto); 
        }

        public async Task AsignarSuscripcionInicial(Guid clienteId, int membresiaId)
        {
            try
            {
                var suscripcion = new Suscripcion
                {
                    ClienteId = clienteId,
                    MembresiaId = membresiaId,
                    FechaInicio = DateTime.UtcNow,
                    FechaFin = DateTime.UtcNow.AddMonths(1),
                    EstadoSuscripcion = EstadoSuscripcion.Activo
                };

                await unitOfWork.Suscripcion.AddAsync(suscripcion);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al asignar suscripción: {Mensaje}", ex.InnerException?.Message ?? ex.Message);
                throw;
            }
        }

        public async Task<ResultadoServicio<Suscripcion>> RenovarSuscripcionAsync(Suscripcion suscripcion, decimal montoPago)
        {
            if (suscripcion == null)
                return ResultadoServicio<Suscripcion>.Fallo("La suscripción no existe.");

            if (suscripcion.Membresia == null)
                return ResultadoServicio<Suscripcion>.Fallo("La suscripción no tiene una membresía asociada.");

            if (montoPago < suscripcion.Membresia.Precio)
                return ResultadoServicio<Suscripcion>.Fallo("El monto ingresado no coincide con el precio de la membresía.");

            // Verifica si aún está vigente
            if (suscripcion.FechaFin > DateTime.UtcNow)
                return ResultadoServicio<Suscripcion>.Fallo("La suscripción actual aún está vigente. No es necesario renovarla.");

            // Asignar nuevas fechas
            suscripcion.FechaInicio = DateTime.UtcNow;
            suscripcion.FechaFin = DateTime.UtcNow.AddMonths(suscripcion.Membresia.Duracion);
            suscripcion.EstadoSuscripcion = EstadoSuscripcion.Activo;

            await unitOfWork.SaveChangesAsync();

            return ResultadoServicio<Suscripcion>.Ok(suscripcion);
        }
    }
}
