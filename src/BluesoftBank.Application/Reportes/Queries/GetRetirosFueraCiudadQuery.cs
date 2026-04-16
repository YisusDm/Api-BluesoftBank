using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using MediatR;

namespace BluesoftBank.Application.Reportes.Queries;

public sealed record GetRetirosFueraCiudadQuery(
    int? Mes = null,
    int? Anio = null) : IRequest<Result<IReadOnlyList<RetiroFueraCiudadDto>>>;

public sealed class GetRetirosFueraCiudadQueryHandler(IReporteRepository reporteRepository)
    : IRequestHandler<GetRetirosFueraCiudadQuery, Result<IReadOnlyList<RetiroFueraCiudadDto>>>
{
    public async Task<Result<IReadOnlyList<RetiroFueraCiudadDto>>> Handle(
        GetRetirosFueraCiudadQuery request,
        CancellationToken cancellationToken)
    {
        var resultado = await reporteRepository.GetRetirosFueraCiudadAsync(
            request.Mes, request.Anio, cancellationToken);

        return Result<IReadOnlyList<RetiroFueraCiudadDto>>.Success(resultado);
    }
}
