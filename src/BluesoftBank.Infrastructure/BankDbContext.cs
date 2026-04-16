using BluesoftBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BluesoftBank.Infrastructure;

public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options)
{
    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Transaccion> Transacciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
