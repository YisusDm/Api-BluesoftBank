using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Queries;

public sealed record GetCuentasQuery(int Pagina = 1, int Tamano = 10)
    : IRequest<Result<CuentasPagedDto>>;

public sealed class GetCuentasQueryValidator : AbstractValidator<GetCuentasQuery>
{
    public GetCuentasQueryValidator()
    {
        RuleFor(x => x.Pagina).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Tamano).InclusiveBetween(1, 100);
    }
}

public sealed class GetCuentasQueryHandler(ICuentaRepository cuentaRepository)
    : IRequestHandler<GetCuentasQuery, Result<CuentasPagedDto>>
{
    public async Task<Result<CuentasPagedDto>> Handle(
        GetCuentasQuery request,
        CancellationToken cancellationToken)
    {
        var (cuentas, totalRegistros) = await cuentaRepository.GetPagedAsync(
            request.Pagina, request.Tamano, cancellationToken);

        return Result<CuentasPagedDto>.Success(
            new CuentasPagedDto(cuentas, totalRegistros, request.Pagina, request.Tamano));
    }
}
