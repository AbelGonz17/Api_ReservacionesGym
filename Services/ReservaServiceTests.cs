using ApiReservacionesGym.DTOs.Reserva;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Migrations;
using ApiReservacionesGym.Respositorio;
using ApiReservacionesGym.Servicios.Metodos;
using ApiReservacionesGym.Servicios.Reserva;
using ApiReservacionesGym.UnitOfWork;
using ApiReservacionesGym.Utilidades.MapeosPersonalizado;
using AutoMapper;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApiReservacioncesGym.Tests.Services
{
    public  class ReservaServiceTests
    {
        [Fact]
        public async Task CrearReserva_DeberiaGuardarCorrectamente()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockReservaRepo = new Mock<IGenericRepository<Reserva, int>>();
            var mockUsuarioActualHelper = new Mock<IUsuarioActualHelper>();
            var mockMapeosPersonalizados = new Mock<IMapeosPersonalizados>();

            mockUnitOfWork.Setup(u => u.Reserva).Returns(mockReservaRepo.Object);
            mockUsuarioActualHelper.Setup(u => u.ObtenerUsuarioId()).Returns(Guid.NewGuid());

            // Simulando el mapeo personalizado
            mockMapeosPersonalizados.Setup(m => m.MapReserva(It.IsAny<CreacionReservaDTO>(), It.IsAny<Clase>(), It.IsAny<Guid>()))
                .Returns(new Reserva { Id = 1, ClienteId = Guid.NewGuid(), ClaseId = 1, Estado = EstadoReserva.Activa });

            // Simulando las llamadas a la base de datos
            // Para GetByConditionAsync
            mockUnitOfWork.Setup(u => u.Suscripcion.GetByConditionAsync(It.IsAny<Expression<Func<Suscripcion, bool>>>(), It.IsAny<string>()))
                .ReturnsAsync(new Suscripcion { ClienteId = Guid.NewGuid(), EstadoSuscripcion = EstadoSuscripcion.Activo });

            // Para GetByAsyncId
            mockUnitOfWork.Setup(u => u.Clase.GetByAsyncId(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Clase { Id = 1, Nombre = "Clase de prueba", CupoMaximo = 2 });

            var servicio = new ReservaService(
                mockUnitOfWork.Object,
                mockMapper.Object,
                mockUsuarioActualHelper.Object,
                mockMapeosPersonalizados.Object
            );

            var creacionDto = new CreacionReservaDTO
            {
                ClaseId = 1
            };

            var reservaEntidad = new Reserva
            {
                Id = 1,
                ClienteId = mockUsuarioActualHelper.Object.ObtenerUsuarioId(),
                ClaseId = creacionDto.ClaseId,
                Estado = EstadoReserva.Activa
            };

            var reservaDtoEsperado = new ReservaDTO
            {
                Id = reservaEntidad.Id,
                ClienteId = reservaEntidad.ClienteId,
                ClaseId = reservaEntidad.ClaseId,
                Estado = reservaEntidad.Estado
            };

            mockMapper.Setup(m => m.Map<Reserva>(creacionDto)).Returns(reservaEntidad);
            mockMapper.Setup(m => m.Map<ReservaDTO>(It.IsAny<Reserva>())).Returns(reservaDtoEsperado);

            mockReservaRepo.Setup(r => r.AddAsync(It.IsAny<Reserva>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var resultado = await servicio.CrearReserva(creacionDto);

            // Assert
            resultado.Exitoso.Should().BeTrue();
            resultado.Datos.Should().BeEquivalentTo(reservaDtoEsperado);

            mockReservaRepo.Verify(r => r.AddAsync(It.IsAny<Reserva>()), Times.Once);
            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ElimarReservaExitosaMente()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockReservaRepo = new Mock<IGenericRepository<Reserva, int>>();
            var mockUsuarioActualHelper = new Mock<IUsuarioActualHelper>();
            var mockMapeosPersonalizados = new Mock<IMapeosPersonalizados>();

            mockUnitOfWork.Setup(u => u.Reserva).Returns(mockReservaRepo.Object);
            mockUsuarioActualHelper.Setup(u => u.ObtenerUsuarioId()).Returns(Guid.NewGuid());

            var servicio = new ReservaService(
                mockUnitOfWork.Object,
                mockMapper.Object,
                mockUsuarioActualHelper.Object,
                mockMapeosPersonalizados.Object
            );

            var reservaId = 1;
            var reservaEntidad = new Reserva
            {
                Id = reservaId,
                ClienteId = Guid.NewGuid(),
                ClaseId = 1,
                Estado = EstadoReserva.Activa
            };

            mockReservaRepo.Setup(r => r.GetByAsyncId(reservaId, It.IsAny<string>()))
                .ReturnsAsync(reservaEntidad);

            mockReservaRepo.Setup(r => r.Delete(reservaEntidad));

            var resultado = await servicio.Delete(reservaId);

            resultado.Exitoso.Should().BeTrue();

            mockReservaRepo.Verify(r => r.Delete(It.IsAny<Reserva>()), Times.Once);

            mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ObtenerLaReservaPorElId()
        {

            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockReservaRepo = new Mock<IGenericRepository<Reserva, int>>();
            var mockUsuarioActualHelper = new Mock<IUsuarioActualHelper>();
            var mockMapeosPersonalizados = new Mock<IMapeosPersonalizados>();

            mockUnitOfWork.Setup(u => u.Reserva).Returns(mockReservaRepo.Object);
            mockUsuarioActualHelper.Setup(u => u.ObtenerUsuarioId()).Returns(Guid.NewGuid());

            var servicio = new ReservaService(
                mockUnitOfWork.Object,
                mockMapper.Object,
                mockUsuarioActualHelper.Object,
                mockMapeosPersonalizados.Object
            );

            var reservaId = 1;
            var reservaEntidad = new Reserva
            {
                Id = reservaId,
                ClienteId = Guid.NewGuid(),
                ClaseId = 1,
                Estado = EstadoReserva.Activa
            };

            mockReservaRepo.Setup(r => r.GetByAsyncId(reservaId, It.IsAny<string>()))
                .ReturnsAsync(reservaEntidad);

            mockMapper.Setup(m => m.Map<DetalleReservaDTO>(It.IsAny<Reserva>())).Returns(new DetalleReservaDTO());

            var resultado = await servicio.GetReservaById(reservaId);
         
            resultado.Exitoso.Should().BeTrue();
            resultado.Datos.Should().NotBeNull();

            mockReservaRepo.Verify(r => r.GetByAsyncId(reservaId, It.IsAny<string>()), Times.Once);
           
        }

        [Fact]
        public async Task ObtenerReservas()
        {
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockReservaRepo = new Mock<IGenericRepository<Reserva, int>>();
            var mockUsuarioActualHelper = new Mock<IUsuarioActualHelper>();
            var mockMapeosPersonalizados = new Mock<IMapeosPersonalizados>();

            mockUnitOfWork.Setup(u => u.Reserva).Returns(mockReservaRepo.Object);
            mockUsuarioActualHelper.Setup(u => u.ObtenerUsuarioId()).Returns(Guid.NewGuid());

            var servicio = new ReservaService(
                mockUnitOfWork.Object,
                mockMapper.Object,
                mockUsuarioActualHelper.Object,
                mockMapeosPersonalizados.Object
            );

            var reservaEntidad = new Reserva
            {
                Id = 1,
                ClienteId = Guid.NewGuid(),
                ClaseId = 1,
                Estado = EstadoReserva.Activa
            };

            var listaReservas = new List<Reserva> { reservaEntidad };

            mockReservaRepo.Setup(r => r.GetAllAsync(It.IsAny<string>()))
                .ReturnsAsync(listaReservas);

            mockMapper.Setup(m => m.Map<List<DetalleReservaDTO>>(It.IsAny<List<Reserva>>())).Returns(new List<DetalleReservaDTO>());

            var resultado = await servicio.ObtenerReservas();

            resultado.Exitoso.Should().BeTrue();

            resultado.Datos.Should().NotBeNull();

        }
    }
}
