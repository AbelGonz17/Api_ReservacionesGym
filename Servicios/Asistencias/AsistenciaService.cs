using ApiReservacionesGym.DTOs.Asistencia;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;

namespace ApiReservacionesGym.Servicios.Asistencias
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUsuarioActualHelper usuarioActualHelper;

        public AsistenciaService(IUnitOfWork unitOfWork, IMapper mapper,IHttpContextAccessor httpContextAccessor,IUsuarioActualHelper usuarioActualHelper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.usuarioActualHelper = usuarioActualHelper;
        }

        public async Task<ResultadoServicio<List<DetalleAsistenciaDTO>>> ObtenerTodasLasAsistencias()
        {
            var asistencia = await unitOfWork.Asistencia.GetAllAsync("Cliente,Clase");
            if (asistencia == null || !asistencia.Any() )
            {
                return ResultadoServicio<List<DetalleAsistenciaDTO>>.Fallo("No se encontraron asistencias");
            }

            var asistenciaDTO = mapper.Map<List<DetalleAsistenciaDTO>>(asistencia);
            return ResultadoServicio<List<DetalleAsistenciaDTO>>.Ok(asistenciaDTO);

        }

        public async Task<ResultadoServicio<DetalleAsistenciaDTO>> ObtenerAsistencia(int id)
        {
            var asistencia = await unitOfWork.Asistencia.GetByAsyncId(id, "Cliente,Clase");
            if (asistencia == null)
            {
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo($"La asistencia con el ID {id} no existe");
            }
            var dto = mapper.Map<DetalleAsistenciaDTO>(asistencia);
            return ResultadoServicio<DetalleAsistenciaDTO>.Ok(dto);
        }

        public async Task<ResultadoServicio<DetalleAsistenciaDTO>> CrearAsistencia(CreacionAsistenciaDTO creacionAsistenciaDTO)
        {
            Guid clienteId;

            try
            {
                clienteId = usuarioActualHelper.ObtenerUsuarioId();
            }
            catch (Exception)
            {
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo("Usuario no autenticado");
            }

            var cliente = await unitOfWork.Cliente.GetByConditionAsync(c => c.Id == clienteId);
            if (cliente == null)
            {
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo("El cliente no existe.");
            }

            var suscripcionActiva = await unitOfWork.Suscripcion
                .GetByConditionAsync(s => s.ClienteId == clienteId && s.EstadoSuscripcion == EstadoSuscripcion.Activo);

            if (suscripcionActiva == null)
            {
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo("No tiene una suscripción activa.");
            }

            var clase = await unitOfWork.Clase.GetByAsyncId(creacionAsistenciaDTO.ClaseId);
            if (clase == null)
            {
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo("La clase no existe.");
            }


            if (!await ClienteTieneReserva(clienteId, creacionAsistenciaDTO.ClaseId))
            {
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo("Debe tener una reserva para esta clase.");
            }


            var asistenciaExistente = await unitOfWork.Asistencia.GetByConditionAsync(a => a.ClienteId == clienteId
            && a.ClaseId == creacionAsistenciaDTO.ClaseId && a.Fecha.Date == DateTime.UtcNow.Date);

            if (asistenciaExistente != null)
            {
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo("Ya se registró una asistencia para esta clase.");
            }

            var asistencia = new Asistencia
            {
                ClienteId = clienteId,
                ClaseId = creacionAsistenciaDTO.ClaseId,
                Fecha = DateTime.UtcNow
            };


            await unitOfWork.Asistencia.AddAsync(asistencia);

            var reserva = await unitOfWork.Reserva
               .GetByConditionAsync(r => r.ClienteId == clienteId && r.ClaseId == clase.Id && r.Estado == EstadoReserva.Activa);

            if (reserva != null)
            {
                reserva.Estado = EstadoReserva.Cancelada;
                unitOfWork.Reserva.Update(reserva);
            }

            using var transaccion = await unitOfWork.BeginTransactionAsync();

            try
            {
                // Guardar asistencia
                // Cancelar reserva
                await unitOfWork.SaveChangesAsync();
                await transaccion.CommitAsync();
            }
            catch
            {
                await transaccion.RollbackAsync();
                return ResultadoServicio<DetalleAsistenciaDTO>.Fallo("Error al guardar asistencia.");
            }

            var asistenciaDTO = new DetalleAsistenciaDTO
            {
                Id = asistencia.Id,
                Fecha = asistencia.Fecha,
                ClienteId = clienteId,
                ClaseId = clase.Id,
                EmailCliente = cliente.Email,
                NombreCliente = cliente.Nombre,
                NombreClase = clase.Nombre,
                Intructor = clase.Intructor
            };

            return ResultadoServicio<DetalleAsistenciaDTO>.Ok(asistenciaDTO);
        }

        private async Task<bool> ClienteTieneReserva(Guid clienteId, int claseId)
        {
            var reservas = await unitOfWork.Reserva.GetByConditionAsync(r => r.ClienteId == clienteId && r.ClaseId == claseId);
            return reservas != null;
        }
    }
}
