using ApiReservacionesGym.DTOs.Clases;
using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Respositorio;
using ApiReservacionesGym.Servicios.Clase;
using ApiReservacionesGym.UnitOfWork;
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
    public class ClaseServiceTests
    {
        [Fact]
        public async Task CrearClaseExitosamente()
        {
            // Arrange
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockMapper = new Mock<IMapper>();
            var mockClaseRepo = new Mock<IGenericRepository<Clase, int>>();

            var mockClaseService = new Mock<ClaseService>(mockUnitOfWork.Object, mockMapper.Object);
            mockUnitOfWork.Setup(u => u.Clase).Returns(mockClaseRepo.Object);

            var servicio = new ClaseService(
                mockUnitOfWork.Object,
                mockMapper.Object
            );  


            var creacionClaseDto = new CreacionClaseDTO
            {
                Nombre = "Zumo",
                Intructor = "Juan Perez",
                Hora = new TimeSpan(10, 0, 0),
                CupoMaximo = 20,
                ClaseDias = new List<string>
                {
                    "Lunes",
                    "Miercoles"
                }
            };

            var clase = new Clase();

            var clasesDto = new ClasesDTO();

            mockMapper.Setup(m=> m.Map<Clase>(creacionClaseDto)).Returns(clase);
            mockMapper.Setup(m => m.Map<ClasesDTO>(clase)).Returns(clasesDto);

            mockClaseRepo.Setup(r => r.GetByConditionAsync(It.IsAny<Expression<Func<Clase, bool>>>(), It.IsAny<string>()))
              .ReturnsAsync((Clase?)null);

            mockClaseRepo.Setup(r => r.GetAllAsync("ClaseDias"))
                         .ReturnsAsync(new List<Clase>()); // No hay conflictos de horario

            var resultado = await servicio.CrearClase(creacionClaseDto);

            // Verificar
            resultado.Exitoso.Should().BeTrue(); // aqui no deberia fallar ya
            resultado.Datos.Should().NotBeNull();


        }
    }
}
