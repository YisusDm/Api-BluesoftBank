using BluesoftBank.Application.Reportes.Queries;

namespace BluesoftBank.Application.Common.Interfaces;

public interface IReporteRepository
{
    Task<IReadOnlyList<TopClienteDto>> GetTopClientesAsync(
        int mes,
        int anio,
        int top,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RetiroFueraCiudadDto>> GetRetirosFueraCiudadAsync(
        int? mes,
        int? anio,
        CancellationToken cancellationToken = default);
}
