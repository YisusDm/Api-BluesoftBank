using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Cuentas.Queries;
using BluesoftBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BluesoftBank.Infrastructure.Persistence.Repositories;

public sealed class CuentaRepository(BankDbContext context) : ICuentaRepository
{
    public async Task<Cuenta?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Cuentas
            .Include(c => c.Transacciones)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    // Aplica UPDLOCK + ROWLOCK para garantizar thread safety en retiros concurrentes.
    // El hint bloquea la fila antes de leer el saldo, evitando que otro hilo
    // lea el mismo saldo stale y produzca doble gasto.
    public async Task<Cuenta?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Cuentas
            .FromSqlInterpolated(
                $"SELECT * FROM Cuentas WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}")
            .Include(c => c.Transacciones)
            .AsTracking()
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

    public async Task<(IReadOnlyList<CuentaListItemDto> Cuentas, int TotalRegistros)> GetPagedAsync(
        int pagina,
        int tamano,
        CancellationToken cancellationToken = default)
    {
        var totalRegistros = await context.Cuentas.CountAsync(cancellationToken);

        // El JOIN debe ir antes de Skip/Take para que EF Core genere SQL válido.
        // Se proyecta a anónimo primero (EF Core lo traduce), luego se mapea a DTO en memoria.
        var rows = await (
            from c in context.Cuentas
            join cl in context.Clientes on c.ClienteId equals cl.Id
            orderby c.FechaCreacion descending
            select new
            {
                c.Id,
                c.NumeroCuenta,
                TipoCuenta = EF.Property<string>(c, "Discriminador"),
                c.Saldo,
                c.FechaCreacion,
                ClienteId = cl.Id,
                cl.Nombre,
                cl.Correo,
                CiudadCliente = cl.Ciudad.Nombre
            })
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(cancellationToken);

        var cuentas = rows.Select(r => new CuentaListItemDto(
            r.Id,
            r.NumeroCuenta,
            r.TipoCuenta,
            r.Saldo,
            r.FechaCreacion,
            new ClienteResumenDto(r.ClienteId, r.Nombre, r.Correo, r.CiudadCliente)))
            .ToList();

        return (cuentas, totalRegistros);
    }
}
