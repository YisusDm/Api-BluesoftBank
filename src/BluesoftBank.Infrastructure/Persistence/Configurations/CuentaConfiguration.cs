using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BluesoftBank.Infrastructure.Persistence.Configurations;

public sealed class CuentaConfiguration : IEntityTypeConfiguration<Cuenta>
{
    public void Configure(EntityTypeBuilder<Cuenta> builder)
    {
        builder.ToTable("Cuentas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.NumeroCuenta)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.NumeroCuenta)
            .IsUnique();

        builder.Property(c => c.Saldo)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(c => c.FechaCreacion)
            .HasColumnType("datetime2(7)")
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.OwnsOne(c => c.Ciudad, ciudad =>
        {
            ciudad.Property(v => v.Nombre)
                .HasColumnName("CiudadNombre")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(c => c.ClienteId).IsRequired();

        builder.HasIndex(c => c.ClienteId);

        // TPH: una sola tabla con discriminador
        builder.HasDiscriminator<string>("Discriminador")
            .HasValue<CuentaAhorro>("CuentaAhorro")
            .HasValue<CuentaCorriente>("CuentaCorriente");

        builder.HasMany(c => c.Transacciones)
            .WithOne()
            .HasForeignKey(t => t.CuentaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(c => c.Transacciones).AutoInclude(false);
    }
}

public sealed class CuentaCorrienteConfiguration : IEntityTypeConfiguration<CuentaCorriente>
{
    public void Configure(EntityTypeBuilder<CuentaCorriente> builder)
    {
        builder.Property(c => c.CupoSobregiro)
            .HasColumnType("decimal(18,2)")
            .HasColumnName("CupoSobregiro");
    }
}
