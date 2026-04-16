using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Reportes.Queries;
using BluesoftBank.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BluesoftBank.Infrastructure.Persistence.Repositories;

public sealed class ReporteRepository(BankDbContext context) : IReporteRepository
{
    public async Task<IReadOnlyList<TopClienteDto>> GetTopClientesAsync(
        int mes,
        int anio,
        int top,
        CancellationToken cancellationToken = default)
    {
        var resultado = await context.Clientes
            .Join(context.Cuentas,
                cliente => cliente.Id,
                cuenta => cuenta.ClienteId,
                (cliente, cuenta) => new { cliente, cuenta })
            .Join(context.Transacciones
                    .Where(t => t.Fecha.Year == anio && t.Fecha.Month == mes),
                cc => cc.cuenta.Id,
                transaccion => transaccion.CuentaId,
                (cc, transaccion) => new { cc.cliente, transaccion })
            .GroupBy(x => new { x.cliente.Id, x.cliente.Nombre })
            .Select(g => new TopClienteDto(
                g.Key.Id,
                g.Key.Nombre,
                g.Count(),
                g.Count(x => x.transaccion.Tipo == TipoTransaccion.Consignacion),
                g.Count(x => x.transaccion.Tipo == TipoTransaccion.Retiro)))
            .OrderByDescending(x => x.TotalTransacciones)
            .Take(top)
            .ToListAsync(cancellationToken);

        return resultado;
    }

    public async Task<IReadOnlyList<RetiroFueraCiudadDto>> GetRetirosFueraCiudadAsync(
        int? mes,
        int? anio,
        CancellationToken cancellationToken = default)
    {
        var transaccionesQuery = context.Transacciones
            .Where(t => t.EsFueraDeCiudadOrigen && t.Tipo == TipoTransaccion.Retiro);

        if (mes.HasValue)
            transaccionesQuery = transaccionesQuery.Where(t => t.Fecha.Month == mes.Value);

        if (anio.HasValue)
            transaccionesQuery = transaccionesQuery.Where(t => t.Fecha.Year == anio.Value);

        var resultado = await context.Clientes
            .Join(context.Cuentas,
                cliente => cliente.Id,
                cuenta => cuenta.ClienteId,
                (cliente, cuenta) => new { cliente, cuenta })
            .Join(transaccionesQuery,
                cc => cc.cuenta.Id,
                transaccion => transaccion.CuentaId,
                (cc, transaccion) => new { cc.cliente, transaccion })
            .GroupBy(x => new { x.cliente.Id, x.cliente.Nombre, CiudadNombre = x.cliente.Ciudad.Nombre })
            .Select(g => new
            {
                g.Key.Id,
                g.Key.Nombre,
                g.Key.CiudadNombre,
                Total = g.Count(),
                ValorTotal = g.Sum(x => x.transaccion.Monto),
                UltimoRetiro = g.Max(x => x.transaccion.Fecha)
            })
            .Where(x => x.ValorTotal > 1_000_000)
            .OrderByDescending(x => x.ValorTotal)
            .Select(x => new RetiroFueraCiudadDto(
                x.Id,
                x.Nombre,
                x.CiudadNombre,
                x.Total,
                x.ValorTotal,
                x.UltimoRetiro))
            .ToListAsync(cancellationToken);

        return resultado;
    }
}
