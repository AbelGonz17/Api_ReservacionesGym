using ApiReservacionesGym.DTOs.Reserva;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.UnitOfWork;
using ApiReservacionesGym.Utilidades.MapeosPersonalizado;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiReservacionesGym.Servicios.Reserva
{
    public class ReservaService : IReservaService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUsuarioActualHelper usuarioActualHelper;
        private readonly IMapeosPersonalizados mapeosPersonalizados;

        public ReservaService(IUnitOfWork unitOfWork, IMapper mapper,IUsuarioActualHelper usuarioActualHelper , IMapeosPersonalizados mapeosPersonalizados)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.usuarioActualHelper = usuarioActualHelper;
            this.mapeosPersonalizados = mapeosPersonalizados;
        }

        public async Task<ResultadoServicio<List<DetalleReservaDTO>>> ObtenerReservas()
        {
            var reservas = await unitOfWork.Reserva.GetAllAsync("Cliente,Cliente.Suscripciones,Clase,Clase.ClaseDias");
            if (reservas == null || !reservas.Any())
            {
                return ResultadoServicio<List<DetalleReservaDTO>>.Fallo("No se encontraron reservas");
            }
            try
            {
                var dto = mapper.Map<List<DetalleReservaDTO>>(reservas);
                return ResultadoServicio<List<DetalleReservaDTO>>.Ok(dto);
            }
            catch (AutoMapperMappingException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
            }

            return ResultadoServicio<List<DetalleReservaDTO>>.Fallo("Error al mapear las reservas");

        }

        public async Task<ResultadoServicio<DetalleReservaDTO>> GetReservaById(int id)
        {
            var reserva = await unitOfWork.Reserva.GetByAsyncId(id, "Cliente,Clase,Clase.ClaseDias");

            if (reserva == null)
            {
                return ResultadoServicio<DetalleReservaDTO>.Fallo($"La reserva con el ID {id} no existe");
            }

            var dto = mapper.Map<DetalleReservaDTO>(reserva);
            return ResultadoServicio<DetalleReservaDTO>.Ok(dto);
        }

        public async Task<ResultadoServicio<ReservaDTO>> CrearReserva(CreacionReservaDTO creacionReservaDTO)
        {
            Guid clienteId;

            try
            {
                clienteId = usuarioActualHelper.ObtenerUsuarioId();
            }
            catch (Exception)
            {
                return ResultadoServicio<ReservaDTO>.Fallo("Usuario no autenticado");
            }

            var suscripcionActiva = await unitOfWork.Suscripcion
                .GetByConditionAsync(s => s.ClienteId == clienteId && s.EstadoSuscripcion == EstadoSuscripcion.Activo);

            if (suscripcionActiva == null)
            {
                return ResultadoServicio<ReservaDTO>.Fallo("El cliente no tiene una suscripción activa.");
            }

            var reservaExistente = await unitOfWork.Reserva
               .GetByConditionAsync(r => r.ClienteId == clienteId && r.ClaseId == creacionReservaDTO.ClaseId);

            if (reservaExistente != null)
            {
                return ResultadoServicio<ReservaDTO>.Fallo("Ya usted tiene una reserva registrada para esta clase.");
            }

            if (await ClienteTieneDemasiadasReservas(clienteId))
            {
                return ResultadoServicio<ReservaDTO>.Fallo("El limite de reservas es 2 por persona y usted cuenta con 2 activas ");
            }

            var clase = await unitOfWork.Clase.GetByAsyncId(creacionReservaDTO.ClaseId);

            if (clase == null)
            {
                return ResultadoServicio<ReservaDTO>.Fallo($"La Clase con el ID {creacionReservaDTO.ClaseId} no se encuentra en el sistema");
            }


            if (await ClaseCupoMaximo(clase.Id))
            {
                return ResultadoServicio<ReservaDTO>.Fallo("La clase ya ha alcanzado su capacidad máxima.");
            }

            var reserva = mapeosPersonalizados.MapReserva(creacionReservaDTO, clase, clienteId);

            await unitOfWork.Reserva.AddAsync(reserva);
            await unitOfWork.SaveChangesAsync();

            var reservaDTO = mapper.Map<ReservaDTO>(reserva);
            return ResultadoServicio<ReservaDTO>.Ok(reservaDTO);
        }

        public async Task<ResultadoServicio<string>> Delete(int id)
        {
            var reservaExiste = await unitOfWork.Reserva.GetByAsyncId(id);
            if (reservaExiste == null)
            {
                return ResultadoServicio<string>.Fallo($"No se encuetra la reserva con el ID {id}");
            }

            unitOfWork.Reserva.Delete(reservaExiste);
            await unitOfWork.SaveChangesAsync();

            return ResultadoServicio<string>.Ok("Su reserva se elimino Exitosamente");
        }

        private async Task<bool> ClienteTieneDemasiadasReservas(Guid clienteId)
        {
            var reservasActivas = await unitOfWork.Reserva
                .ContarAsync(x => x.ClienteId == clienteId && x.Estado == EstadoReserva.Activa);

            return reservasActivas >= 2;
        }

        private async Task<bool> ClaseCupoMaximo(int claseId)
        {
            var reservasClase = await unitOfWork.Reserva
             .ContarAsync(x => x.ClaseId == claseId && x.Estado == EstadoReserva.Activa);

            return reservasClase >= 2;

        }
    }

}
