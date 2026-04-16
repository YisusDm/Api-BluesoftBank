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
        // Query syntax: EF Core traduce joins y GroupBy de forma más confiable.
        // Sum(condicion ? 1 : 0) se emite como SUM(CASE WHEN ... THEN 1 ELSE 0 END).
        var resultado = await (
            from t in context.Transacciones
            where t.Fecha.Year == anio && t.Fecha.Month == mes
            join c in context.Cuentas on t.CuentaId equals c.Id
            join cl in context.Clientes on c.ClienteId equals cl.Id
            group new { t.Tipo } by new { cl.Id, cl.Nombre } into g
            orderby g.Count() descending
            select new TopClienteDto(
                g.Key.Id,
                g.Key.Nombre,
                g.Count(),
                g.Sum(x => x.Tipo == TipoTransaccion.Consignacion ? 1 : 0),
                g.Sum(x => x.Tipo == TipoTransaccion.Retiro ? 1 : 0)))
            .Take(top)
            .ToListAsync(cancellationToken);

        return resultado;
    }

    public async Task<IReadOnlyList<RetiroFueraCiudadDto>> GetRetirosFueraCiudadAsync(
        int? mes,
        int? anio,
        CancellationToken cancellationToken = default)
    {
        // Construir el filtro de transacciones antes del join para que EF Core
        // lo incorpore en el WHERE del SQL generado.
        var transacciones = context.Transacciones
            .Where(t => t.EsFueraDeCiudadOrigen && t.Tipo == TipoTransaccion.Retiro);

        if (mes.HasValue)
            transacciones = transacciones.Where(t => t.Fecha.Month == mes.Value);

        if (anio.HasValue)
            transacciones = transacciones.Where(t => t.Fecha.Year == anio.Value);

        var resultado = await (
            from t in transacciones
            join c in context.Cuentas on t.CuentaId equals c.Id
            join cl in context.Clientes on c.ClienteId equals cl.Id
            group new { t.Monto, t.Fecha } by new
            {
                cl.Id,
                cl.Nombre,
                CiudadNombre = cl.Ciudad.Nombre
            } into g
            where g.Sum(x => x.Monto) > 1_000_000
            orderby g.Sum(x => x.Monto) descending
            select new RetiroFueraCiudadDto(
                g.Key.Id,
                g.Key.Nombre,
                g.Key.CiudadNombre,
                g.Count(),
                g.Sum(x => x.Monto),
                g.Max(x => x.Fecha)))
            .ToListAsync(cancellationToken);

        return resultado;
    }
}
