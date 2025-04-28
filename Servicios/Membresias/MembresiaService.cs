using ApiReservacionesGym.DTOs.Clases;
using ApiReservacionesGym.DTOs.MembresiaDTO;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;

namespace ApiReservacionesGym.Servicios.Membresias
{
    public class MembresiaService : IMembresiaService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public MembresiaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ResultadoServicio<List<MembresiaDTO>>> ObtenerTodasLasMembresias()
        {
            var membresia = await unitOfWork.Membresia.GetAllAsync();

            if (membresia == null)
            {
                return ResultadoServicio<List<MembresiaDTO>>.Fallo("No se encontraron Membresias");
            }

            var membresiaDTO = mapper.Map<List<MembresiaDTO>>(membresia);
            return ResultadoServicio<List<MembresiaDTO>>.Ok(membresiaDTO);
        }

        public async Task<ResultadoServicio<MembresiaDTO>> ObtenerMembresiaPorId(int id)
        {
            var membresia = await unitOfWork.Membresia.GetByAsyncId(id);
            if (membresia == null)
            {
                return ResultadoServicio<MembresiaDTO>.Fallo($"La membresía con el ID {id} no existe");
            }
            var dto = mapper.Map<MembresiaDTO>(membresia);
            return ResultadoServicio<MembresiaDTO>.Ok(dto);
        }

        public async Task<ResultadoServicio<MembresiaDTO>> CrearMembresia(CreacionMembresiaDTO creacionMembresiaDTO)
        {
            if (!Enum.IsDefined(typeof(TipoMembresia), creacionMembresiaDTO.Tipo))
            {
                return ResultadoServicio<MembresiaDTO>.Fallo("El tipo de membresía no es válido.");
            }

            var membresiaExistente = await unitOfWork.Membresia
                .GetByConditionAsync(m => m.Tipo == creacionMembresiaDTO.Tipo);
            if (membresiaExistente != null)
            {
                return ResultadoServicio<MembresiaDTO>.Fallo("Ya existe una membresía de ese tipo.");
            }

            var membresia = mapper.Map<Membresia>(creacionMembresiaDTO);

            await unitOfWork.Membresia.AddAsync(membresia);
            await unitOfWork.SaveChangesAsync();

            var dto = mapper.Map<MembresiaDTO>(membresia);
            return ResultadoServicio<MembresiaDTO>.Ok(dto);
        }

        public async Task<ResultadoServicio<MembresiaDTO>> ActualizarMembresia(int id, JsonPatchDocument<MembresiaPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return ResultadoServicio<MembresiaDTO>.Fallo("El documento de parcheo no puede ser nulo");
            }

            var membresiaDb = await unitOfWork.Membresia.GetByAsyncId(id);
            if (membresiaDb == null)
            {
                return ResultadoServicio<MembresiaDTO>.Fallo($"La membresia con el ID {id} no existe");
            }

            var membresiaDTO = mapper.Map<MembresiaPatchDTO>(membresiaDb);

            try
            {
                patchDocument.ApplyTo(membresiaDTO);
            }
            catch (Exception ex)
            {
                return ResultadoServicio<MembresiaDTO>.Fallo($"Error al aplicar el parche: {ex.Message}");
            }

            mapper.Map(membresiaDTO, membresiaDb);

            await unitOfWork.SaveChangesAsync();
            var membresiaActualizada = mapper.Map<MembresiaDTO>(membresiaDb);
            if (membresiaActualizada == null)
            {
                return ResultadoServicio<MembresiaDTO>.Fallo("Error al mapear la clase actualizada");
            }

            return ResultadoServicio<MembresiaDTO>.Ok(membresiaActualizada);

        }

        public async Task<ResultadoServicio<string>> Delete(int id)
        {
            var memebresia = await unitOfWork.Membresia.GetByAsyncId(id);
            if (memebresia == null)
            {
                return ResultadoServicio<string>.Fallo($"No se encuetra la membresia con el ID {id}");
            }

            unitOfWork.Membresia.Delete(memebresia);
            await unitOfWork.SaveChangesAsync();

            return ResultadoServicio<string>.Ok("Su Membresia se elimino correctamente");
        }

        public async Task<ResultadoServicio<string>> ExpirarSuscripcion(string usuarioId)
        {
            if (!Guid.TryParse(usuarioId, out var clienteId))
            {
                return ResultadoServicio<string>.Fallo("El ID de usuario no es válido.");
            }

            // Buscar suscripción activa del cliente
            var suscripcion = await unitOfWork.Suscripcion
                .GetByConditionAsync(s => s.ClienteId == clienteId && s.EstadoSuscripcion == EstadoSuscripcion.Activo,"Membresia");

            if (suscripcion == null)
            {
                return ResultadoServicio<string>.Fallo("El cliente no tiene una suscripción activa.");
            }

            // Simular expiración: retrocedemos la fecha fin
            suscripcion.FechaInicio = DateTime.UtcNow.AddMonths(-suscripcion.Membresia.Duracion).AddDays(-1);
            suscripcion.FechaFin = suscripcion.FechaInicio;
            suscripcion.EstadoSuscripcion = EstadoSuscripcion.Inactivo;

            await unitOfWork.SaveChangesAsync();

            return ResultadoServicio<string>.Ok("La suscripción ha sido expirada correctamente.");
        }
    }
}
