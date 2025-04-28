using ApiReservacionesGym.DTOs.Clases;
using ClaseEntidad = ApiReservacionesGym.Entidades.Clase;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.UnitOfWork;
using AutoMapper;
using ApiReservacionesGym.Entidades;
using Microsoft.AspNetCore.JsonPatch;
using System.ComponentModel.DataAnnotations;

namespace ApiReservacionesGym.Servicios.Clase
{
    public class ClaseService : IClaseService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ClaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<ResultadoServicio<List<ClasesDTO>>> ObtenerTodasLasClases()
        {
            var clases = await unitOfWork.Clase.GetAllAsync("ClaseDias");
            if (clases == null || !clases.Any())
            {
                return ResultadoServicio<List<ClasesDTO>>.Fallo("No se encontraron clases");
            }
            var clasesDTO = mapper.Map<List<ClasesDTO>>(clases);
            return ResultadoServicio<List<ClasesDTO>>.Ok(clasesDTO);
        }

        public async Task<ResultadoServicio<ClasesDTO>> ObtenerClasePorId(int id)
        {
            var clase = await unitOfWork.Clase.GetByAsyncId(id,"ClaseDias");

            if (clase == null)
            {
                return ResultadoServicio<ClasesDTO>.Fallo($"La clase con el ID {id} no existe");
            }

            var dto = mapper.Map<ClasesDTO>(clase);
            return ResultadoServicio<ClasesDTO>.Ok(dto);
        }
        
        public async Task<ResultadoServicio<ClasesDTO>> CrearClase(CreacionClaseDTO creacionClaseDTO)
        {
            var claseExistente = await ClaseExistente(creacionClaseDTO.Nombre);
            if (claseExistente)
            {
                return ResultadoServicio<ClasesDTO>.Fallo("Esta clase ya existe");
            }

            var diasEnums = ConvertirDias(creacionClaseDTO.ClaseDias);
            if (diasEnums == null)
            {
                return ResultadoServicio<ClasesDTO>.Fallo("Los días de la semana no son válidos");
            }

            var hayConflicto = await ExisteConflictoHorario(creacionClaseDTO.Hora, diasEnums);
            if (hayConflicto)
            {
                return ResultadoServicio<ClasesDTO>.Fallo("Ya existe una clase programada a esta hora en los días seleccionados");
            }

            var clase = mapper.Map<ClaseEntidad>(creacionClaseDTO);
            clase.ClaseDias = diasEnums.Select(dia => new ClaseDia { Dia = dia }).ToList();

            await unitOfWork.Clase.AddAsync(clase);
            await unitOfWork.SaveChangesAsync();
            var claseDTO = mapper.Map<ClasesDTO>(clase);
            return ResultadoServicio<ClasesDTO>.Ok(claseDTO);
        }

        public async Task<ResultadoServicio<string>> Delete(int id)
        {
            var claseExiste = await unitOfWork.Clase.GetByAsyncId(id);
            if (claseExiste == null)
            {
                return ResultadoServicio<string>.Fallo($"No se encuetra la reserva con el ID {id}");
            }

            unitOfWork.Clase.Delete(claseExiste);
            await unitOfWork.SaveChangesAsync();

            return ResultadoServicio<string>.Ok("Su clase se elimino Exitosamente");
        }



        public async Task<ResultadoServicio<ClasesDTO>> EditarClasePatch(int id, JsonPatchDocument<ClasePatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return ResultadoServicio<ClasesDTO>.Fallo("El documento de parcheo no puede ser nulo");
            }
            var claseDb = await unitOfWork.Clase.GetByAsyncId(id, "ClaseDias");
            if (claseDb == null)
            {
                return ResultadoServicio<ClasesDTO>.Fallo($"La clase con el ID {id} no existe");
            }
            var claseDTO = mapper.Map<ClasePatchDTO>(claseDb);

            try
            {
                patchDocument.ApplyTo(claseDTO);
            }
            catch (Exception ex)
            {
                return ResultadoServicio<ClasesDTO>.Fallo($"Error al aplicar el parche: {ex.Message}");
            }

            bool claseDiasFueModificado = patchDocument.Operations
                .Any(op => op.path.Equals("/claseDias", StringComparison.OrdinalIgnoreCase));

            var validacion = ValidarClaseDTO(claseDTO);
            if (!validacion.Exitoso)
            {
                return ResultadoServicio<ClasesDTO>.Fallo(validacion.Mensaje);
            }

            List<DiaSemana> diasEnums;
          

            if (claseDiasFueModificado)
            {
                diasEnums = ConvertirDias(claseDTO.ClaseDias);

                if (!diasEnums.Any())
                {
                    return ResultadoServicio<ClasesDTO>.Fallo("Los días de la semana no son válidos");
                }

                if (DiasDiferentes(claseDb.ClaseDias.Select(cd => cd.Dia).ToList(), diasEnums))
                {
                    claseDb.ClaseDias = diasEnums.Select(d => new ClaseDia { Dia = d }).ToList();
                }
            }
            else
            {
                diasEnums = claseDb.ClaseDias.Select(cd => cd.Dia).ToList();
            }


            var conflicto = await ExisteConflictoHorarioEditando(claseDb.Id, claseDTO.Hora, diasEnums);
            if (conflicto)
            {
                return ResultadoServicio<ClasesDTO>.Fallo("Ya existe una clase programada a esta hora en los días seleccionados");
            }

            mapper.Map(claseDTO, claseDb);          

            await unitOfWork.SaveChangesAsync();     
            var claseActualizada = mapper.Map<ClasesDTO>(claseDb);
            if (claseActualizada == null)
            {
                return ResultadoServicio<ClasesDTO>.Fallo("Error al mapear la clase actualizada");
            }

            return ResultadoServicio<ClasesDTO>.Ok(claseActualizada);
        }



        private ResultadoServicio<ClasesDTO> ValidarClaseDTO(ClasePatchDTO claseDTO)
        {
            var context = new ValidationContext(claseDTO);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(claseDTO, context, validationResults, true);

            if (!isValid)
            {
                var errores = string.Join(" | ", validationResults.Select(e => e.ErrorMessage));
                return ResultadoServicio<ClasesDTO>.Fallo($"Errores de validación: {errores}");
            }

            return ResultadoServicio<ClasesDTO>.Ok(null); // No errors
        }



        private bool DiasDiferentes(List<DiaSemana> diasActuales, List<DiaSemana> diasNuevos)
        {
            return !diasActuales.OrderBy(d => d).SequenceEqual(diasNuevos.OrderBy(d => d));
        }



        private async Task<bool> ClaseExistente(string nombreClase)
        {
            var clase = await unitOfWork.Clase.GetByConditionAsync(x => x.Nombre == nombreClase);
            return clase != null;
        }



        private List<DiaSemana> ConvertirDias(List<string> dias)
        {
            if (dias == null || !dias.Any())
            {
                return new List<DiaSemana>();
            }

            var diasConvertidos = new List<DiaSemana>();

            foreach (var dia in dias)
            {
                if (Enum.TryParse<DiaSemana>(dia, true, out var diaEnum))
                {
                    diasConvertidos.Add(diaEnum);
                }
            }

            return diasConvertidos;
        }



        private async Task<bool> ExisteConflictoHorario(TimeSpan hora, List<DiaSemana> dias)
        {
            var clases = await unitOfWork.Clase.GetAllAsync("ClaseDias");

            return clases.Any(c => c.Hora == hora &&
                                   c.ClaseDias.Any(cd => dias.Contains(cd.Dia)));
        }



        private async Task<bool> ExisteConflictoHorarioEditando(int idClase, TimeSpan hora, List<DiaSemana> dias)
        {
            var clases = await unitOfWork.Clase.GetAllAsync("ClaseDias");

            return clases.Any(c => c.Id != idClase &&
                                   c.Hora == hora &&
                                   c.ClaseDias.Any(cd => dias.Contains(cd.Dia)));
        }
    }
}
