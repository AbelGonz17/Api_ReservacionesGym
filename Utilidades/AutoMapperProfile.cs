using ApiReservacionesGym.DTOs.Asistencia;
using ApiReservacionesGym.DTOs.Clases;
using ApiReservacionesGym.DTOs.Clientes;
using ApiReservacionesGym.DTOs.MembresiaDTO;
using ApiReservacionesGym.DTOs.Pago;
using ApiReservacionesGym.DTOs.Reserva;
using ApiReservacionesGym.DTOs.Suscripcion;
using ApiReservacionesGym.DTOs.SuscripcionDTO;
using ApiReservacionesGym.Entidades;
using AutoMapper;

namespace ApiReservacionesGym.Utilidades
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Suscripcion
            CreateMap<Suscripcion, SuscripcionDetalleDTO>();
            CreateMap<Cliente, SuscripcionDetalleDTO>();
            CreateMap<Cliente, SuscripcionClienteDTO>();
            CreateMap<Membresia, SuscripcionMembresiaDTO>();
            CreateMap<Suscripcion, SuscripcionDTOs>();

            //Cliente
            CreateMap<Cliente, ClienteDTO>().ReverseMap();
            CreateMap<Membresia, ClienteMembresiaDTO>();

            //Membresia
            CreateMap<Membresia, MembresiaDTO>();
            CreateMap<CreacionMembresiaDTO, Membresia>();
            CreateMap<MembresiaPatchDTO, Membresia>().ReverseMap();

            //Clases
            CreateMap<Clase, ClasesDTO>()
                .ForMember(dest => dest.ClaseDias, opt => opt.MapFrom(src => src.ClaseDias.Select(cd => cd.Dia.ToString()).ToList()));
            CreateMap<CreacionClaseDTO, Clase>()
                .ForMember(x => x.ClaseDias, opciones => opciones.MapFrom(MapClaseDias));
            CreateMap<ClasePatchDTO, Clase>()
                .ForMember(dest => dest.ClaseDias, opt => opt.Ignore())
                .ReverseMap();

            //Reserva
            CreateMap<Reserva, ReservaDTO>();
            CreateMap<Clase, ClaseReservaDTO>()
                .ForMember(dest => dest.ClaseDias,
                    opt => opt.MapFrom(src => src.ClaseDias.Select(cd => cd.Dia).ToList()));

            CreateMap<Reserva, DetalleReservaDTO>()
                .ForMember(dest => dest.Cliente, opt => opt.MapFrom(src => src.Cliente));
            CreateMap<Cliente, ClienteReservaDTO>();

           
            //Asistencia
            CreateMap<Asistencia, AsistenciaDTO>();
            CreateMap<CreacionAsistenciaDTO, Asistencia>();
            CreateMap<Asistencia, DetalleAsistenciaDTO>()
            .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src => src.Cliente.Nombre))
            .ForMember(dest => dest.EmailCliente, opt => opt.MapFrom(src => src.Cliente.Email))
            .ForMember(dest => dest.NombreClase, opt => opt.MapFrom(src => src.Clase.Nombre))
            .ForMember(dest => dest.Intructor, opt => opt.MapFrom(src => src.Clase.Intructor))
            .ForMember(dest => dest.Fecha, opt => opt.MapFrom(src => src.Fecha));


            //Pago
            CreateMap<Pago, PagoDTO>();


        }

        private static List<ClaseDia> MapClaseDias(CreacionClaseDTO creacionClaseDTO, Clase clase)
        {
            var resultado = new List<ClaseDia>();

            if (creacionClaseDTO.ClaseDias == null)
                return resultado;

            foreach (var clasedia in creacionClaseDTO.ClaseDias)
            {
                if (Enum.TryParse<DiaSemana>(clasedia, ignoreCase: true, out var diaEnum))
                {
                    resultado.Add(new ClaseDia
                    {
                        Dia = diaEnum,
                        Clase = clase
                    });
                }
                else
                {                
                    throw new ArgumentException($"El valor '{clasedia}' no es un día válido.");
                }
            }

            return resultado;
        }
    }
}
