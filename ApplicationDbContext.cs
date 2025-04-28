using ApiReservacionesGym.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Numerics;
using System.Reflection.Emit;

namespace ApiReservacionesGym
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Membresia>()
              .Property(p => p.Precio)
              .HasColumnType("decimal(18,2)");

            builder.Entity<Pago>()
              .Property(p => p.Monto)
              .HasColumnType("decimal(18,2)");

            builder.Entity<Suscripcion>()
                .HasOne(s => s.Membresia)
                .WithMany()
                .HasForeignKey(s => s.MembresiaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cliente>()
                .HasMany(s => s.Suscripciones)
                .WithOne(s => s.Cliente)
                .HasForeignKey(s => s.ClienteId);
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Membresia> Membresias { get; set; }
        public DbSet<Clase> Clases { get; set; }
        public DbSet<ClaseDia> ClaseDias { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }

    }
}
