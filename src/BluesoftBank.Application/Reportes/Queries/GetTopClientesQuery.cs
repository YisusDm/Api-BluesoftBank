using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Reportes.Queries;

public sealed record GetTopClientesQuery(
    int Mes,
    int Anio,
    int Top = 10) : IRequest<Result<IReadOnlyList<TopClienteDto>>>;

public sealed class GetTopClientesQueryValidator : AbstractValidator<GetTopClientesQuery>
{
    public GetTopClientesQueryValidator()
    {
        RuleFor(x => x.Mes).InclusiveBetween(1, 12);
        RuleFor(x => x.Anio).GreaterThanOrEqualTo(2020);
        RuleFor(x => x.Top).InclusiveBetween(1, 100);
    }
}

public sealed class GetTopClientesQueryHandler(IReporteRepository reporteRepository)
    : IRequestHandler<GetTopClientesQuery, Result<IReadOnlyList<TopClienteDto>>>
{
    public async Task<Result<IReadOnlyList<TopClienteDto>>> Handle(
        GetTopClientesQuery request,
        CancellationToken cancellationToken)
    {
        var resultado = await reporteRepository.GetTopClientesAsync(
            request.Mes, request.Anio, request.Top, cancellationToken);

        return Result<IReadOnlyList<TopClienteDto>>.Success(resultado);
    }
}
