using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BluesoftBank.Infrastructure.Persistence.Repositories;

public sealed class CuentaRepository(BankDbContext context) : ICuentaRepository
{
    public async Task<Cuenta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Cuentas
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    // Aplica UPDLOCK + ROWLOCK para garantizar thread safety en retiros concurrentes.
    // El hint bloquea la fila antes de leer el saldo, evitando que otro hilo
    // lea el mismo saldo stale y produzca doble gasto.
    public async Task<Cuenta?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Cuentas
            .FromSqlRaw(
                "SELECT * FROM Cuentas WITH (UPDLOCK, ROWLOCK) WHERE Id = {0}",
                id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(Cuenta cuenta, CancellationToken cancellationToken = default)
    {
        await context.Cuentas.AddAsync(cuenta, cancellationToken);
    }

    public void Update(Cuenta cuenta)
    {
        context.Cuentas.Update(cuenta);
    }
}
