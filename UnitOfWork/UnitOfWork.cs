using ApiReservacionesGym.Entidades;
using ApiReservacionesGym.Respositorio;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApiReservacionesGym.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        public IGenericRepository<Reserva,int> Reserva { get; }
        public IGenericRepository<Cliente,Guid> Cliente { get; }
        public IGenericRepository<Clase, int> Clase { get; }
        public IGenericRepository<Membresia, int> Membresia { get; }
        public IGenericRepository<Asistencia, int> Asistencia { get; }
        public IGenericRepository<Pago, int> Pago { get; }
        public IGenericRepository<Suscripcion, int> Suscripcion { get; }

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            Reserva = new GenericRepository<Reserva,int>(context);
            Cliente = new GenericRepository<Cliente,Guid>(context);
            Clase = new GenericRepository<Clase, int>(context);
            Membresia = new GenericRepository<Membresia, int>(context);
            Asistencia = new GenericRepository<Asistencia, int>(context);
            Pago = new GenericRepository<Pago, int>(context);
            Suscripcion= new GenericRepository<Suscripcion, int>(context);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }
    }
}
