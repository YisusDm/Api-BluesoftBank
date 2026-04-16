using BluesoftBank.Domain.Entities;

namespace BluesoftBank.Application.Common.Interfaces;

public interface ITransaccionRepository
{
    Task<IReadOnlyList<Transaccion>> GetByCuentaIdAsync(
        Guid cuentaId,
        int pagina,
        int tamano,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Transaccion>> GetByCuentaIdYPeriodoAsync(
        Guid cuentaId,
        int mes,
        int anio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retorna el SaldoResultante de la última transacción anterior al inicio del período indicado,
    /// o null si no existe ninguna transacción previa (cuenta sin movimientos anteriores al período).
    /// </summary>
    Task<decimal?> GetSaldoAntesDePeriodoAsync(
        Guid cuentaId,
        int mes,
        int anio,
        CancellationToken cancellationToken = default);
}
