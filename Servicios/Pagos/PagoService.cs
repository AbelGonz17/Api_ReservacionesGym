using ApiReservacionesGym.DTOs.Asistencia;
using ApiReservacionesGym.DTOs.AuthDTO;
using ApiReservacionesGym.DTOs.Clientes;
using ApiReservacionesGym.DTOs.Pago;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.Servicios.Suscripciones;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ApiReservacionesGym.Servicios.Pagos
{
    public class PagoService : IPagoService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUsuarioActualHelper usuarioActualHelper;
        private readonly ISuscripcionesServicio suscripcionesServicio;

        public PagoService(IUnitOfWork unitOfWork, IMapper mapper,IUsuarioActualHelper usuarioActualHelper ,ISuscripcionesServicio suscripcionesServicio)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.usuarioActualHelper = usuarioActualHelper;
            this.suscripcionesServicio = suscripcionesServicio;
        }

        public async Task<ResultadoServicio<List<PagoDTO>>> ObtenerPagos()
        {
            var pago = await unitOfWork.Pago.GetAllAsync();
            if (pago == null)
            {
                return ResultadoServicio<List<PagoDTO>>.Fallo("No se encontraron pagos registrados");
            }

            var pagoDTO = mapper.Map<List<PagoDTO>>(pago);
            return ResultadoServicio<List<PagoDTO>>.Ok(pagoDTO);
        }

        public async Task<ResultadoServicio<PagoDTO>> ObtenerPago(int id)
        {
            var pago = await unitOfWork.Pago.GetByAsyncId(id);
            if (pago == null)
            {
                return ResultadoServicio<PagoDTO>.Fallo($"El pago con el ID {id} no existe");
            }

            var dto = mapper.Map<PagoDTO>(pago);
            return ResultadoServicio<PagoDTO>.Ok(dto);
        }

        public async Task<ResultadoServicio<string>> RealizarPago(CreacionPagoDTO creacionPagoDTO)
        {
            Guid clienteId;

            try
            {
                clienteId = usuarioActualHelper.ObtenerUsuarioId();
            }
            catch
            {
                return ResultadoServicio<string>.Fallo("Usuario no autenticado.");
            }

            var suscripcionActiva = await unitOfWork.Suscripcion
                .GetByConditionAsync(s => s.ClienteId == clienteId,"Membresia" );

            if (!Enum.TryParse<MetodosDePago>(creacionPagoDTO.MetodoDePago, true, out var metodoDePago))
                return ResultadoServicio<string>.Fallo("Método de pago inválido.");

            //con este using lo que hacemos es que al final de nuestra transaccion
            //no importa cual sea el resultado, llame a dispose(); para cerrar cualquier
            //conexion
            using var transaccion = await unitOfWork.BeginTransactionAsync();

            try
            {
                // Renovar suscripción
                var resultadoRenovacion = await suscripcionesServicio.RenovarSuscripcionAsync(suscripcionActiva, creacionPagoDTO.Monto);

                if (!resultadoRenovacion.Exitoso)
                    return ResultadoServicio<string>.Fallo(resultadoRenovacion.Mensaje);

                // Registrar el pago
                var pago = new Pago
                {
                    ClienteId = clienteId,
                    Monto = creacionPagoDTO.Monto,
                    Fecha = DateTime.UtcNow,
                    MetodoDePago = metodoDePago
                };

                await unitOfWork.Pago.AddAsync(pago);
                await unitOfWork.SaveChangesAsync();

                await transaccion.CommitAsync();

                return ResultadoServicio<string>.Ok(
                    $"Pago realizado con éxito. Su suscripción está activa hasta el {resultadoRenovacion.Datos.FechaFin:dd/MM/yyyy}.");
            }
            catch (Exception)
            {
                await transaccion.RollbackAsync();
                return ResultadoServicio<string>.Fallo("Ocurrió un error al procesar el pago.");
            }
        }
    }
}
