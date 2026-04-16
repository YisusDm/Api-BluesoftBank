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
}
