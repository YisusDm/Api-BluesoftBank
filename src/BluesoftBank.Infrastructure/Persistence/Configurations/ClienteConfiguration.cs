using BluesoftBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BluesoftBank.Infrastructure.Persistence.Configurations;

public sealed class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Correo)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(c => c.Correo)
            .IsUnique();

        builder.OwnsOne(c => c.Ciudad, ciudad =>
        {
            ciudad.Property(v => v.Nombre)
                .HasColumnName("CiudadNombre")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.HasDiscriminator<string>("Discriminador")
            .HasValue<PersonaNatural>("PersonaNatural")
            .HasValue<Empresa>("Empresa");
    }
}

public sealed class PersonaNaturalConfiguration : IEntityTypeConfiguration<PersonaNatural>
{
    public void Configure(EntityTypeBuilder<PersonaNatural> builder)
    {
        builder.Property(p => p.Cedula)
            .HasMaxLength(20)
            .HasColumnName("Cedula");

        builder.HasIndex(p => p.Cedula)
            .IsUnique();
    }
}

public sealed class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
{
    public void Configure(EntityTypeBuilder<Empresa> builder)
    {
        builder.Property(e => e.Nit)
            .HasMaxLength(20)
            .HasColumnName("Nit");

        builder.HasIndex(e => e.Nit)
            .IsUnique();
    }
}
