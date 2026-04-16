using BluesoftBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BluesoftBank.Infrastructure.Persistence.Configurations;

public sealed class TransaccionConfiguration : IEntityTypeConfiguration<Transaccion>
{
    public void Configure(EntityTypeBuilder<Transaccion> builder)
    {
        builder.ToTable("Transacciones");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.CuentaId).IsRequired();

        builder.Property(t => t.Tipo)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.Monto)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.SaldoResultante)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.OwnsOne(t => t.Ciudad, ciudad =>
        {
            ciudad.Property(v => v.Nombre)
                .HasColumnName("CiudadNombre")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(t => t.EsFueraDeCiudadOrigen).IsRequired();

        builder.Property(t => t.Fecha)
            .HasColumnType("datetime2(7)")
            .IsRequired();

        // Índice para movimientos por cuenta ordenados por fecha
        builder.HasIndex(t => new { t.CuentaId, t.Fecha });

        // Índice para el reporte de retiros fuera de ciudad
        builder.HasIndex(t => new { t.EsFueraDeCiudadOrigen, t.Fecha });
    }
}
