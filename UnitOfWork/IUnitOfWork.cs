using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Respositorio;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApiReservacionesGym.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<Reserva,int> Reserva { get; }
        IGenericRepository<Cliente,Guid> Cliente { get; }
        IGenericRepository<Clase,int> Clase { get; }
        IGenericRepository<Membresia,int> Membresia { get; }
        IGenericRepository<Asistencia,int> Asistencia { get; }
        IGenericRepository<Pago,int> Pago { get; }
        IGenericRepository<Suscripcion, int> Suscripcion { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<int> SaveChangesAsync();
    }
}