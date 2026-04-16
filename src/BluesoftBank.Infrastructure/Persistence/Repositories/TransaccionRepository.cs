using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BluesoftBank.Infrastructure.Persistence.Repositories;

public sealed class TransaccionRepository(BankDbContext context) : ITransaccionRepository
{
    public async Task AddAsync(Transaccion transaccion, CancellationToken cancellationToken = default)
    {
        await context.Transacciones.AddAsync(transaccion, cancellationToken);
    }

    public async Task<IReadOnlyList<Transaccion>> GetByCuentaIdAsync(
        Guid cuentaId,
        int pagina,
        int tamano,
        CancellationToken cancellationToken = default)
    {
        return await context.Transacciones
            .Where(t => t.CuentaId == cuentaId)
            .OrderByDescending(t => t.Fecha)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Transaccion>> GetByCuentaIdYPeriodoAsync(
        Guid cuentaId,
        int mes,
        int anio,
        CancellationToken cancellationToken = default)
    {
        return await context.Transacciones
            .Where(t => t.CuentaId == cuentaId
                && t.Fecha.Year == anio
                && t.Fecha.Month == mes)
            .OrderBy(t => t.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal?> GetSaldoAntesDePeriodoAsync(
        Guid cuentaId,
        int mes,
        int anio,
        CancellationToken cancellationToken = default)
    {
        var fechaInicio = new DateTime(anio, mes, 1);
        return await context.Transacciones
            .Where(t => t.CuentaId == cuentaId && t.Fecha < fechaInicio)
            .OrderByDescending(t => t.Fecha)
            .Select(t => t.SaldoResultante)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
