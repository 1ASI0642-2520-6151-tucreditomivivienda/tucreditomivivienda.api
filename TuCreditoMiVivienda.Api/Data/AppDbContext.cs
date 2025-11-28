using Microsoft.EntityFrameworkCore;
using TuCreditoMiVivienda.Api.CustomerContext.Domain;
using TuCreditoMiVivienda.Api.LoanContext.Domain;

namespace TuCreditoMiVivienda.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<LoanSimulation> LoanSimulations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Client
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombres).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Documento).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        // Configuración de Property
        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Proyecto).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Torre).HasMaxLength(50);
            entity.Property(e => e.Numero).HasMaxLength(50);
            entity.Property(e => e.Ciudad).HasMaxLength(100);
            entity.Property(e => e.TipoUnidad).HasMaxLength(100);
            entity.Property(e => e.Estado).HasMaxLength(50).HasDefaultValue("disponible");
        });

        // Configuración de LoanSimulation
        modelBuilder.Entity<LoanSimulation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Currency).HasMaxLength(10).HasDefaultValue("PEN");
            entity.Property(e => e.RateType).HasMaxLength(20).HasDefaultValue("efectiva");
            entity.Property(e => e.Capitalization).HasMaxLength(20).HasDefaultValue("mensual");
            entity.Property(e => e.GraceType).HasMaxLength(20).HasDefaultValue("sin");
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}

