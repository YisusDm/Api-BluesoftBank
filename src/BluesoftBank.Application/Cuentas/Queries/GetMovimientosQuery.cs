using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Queries;

public sealed record GetMovimientosQuery(
    Guid CuentaId,
    int Pagina = 1,
    int Tamano = 20) : IRequest<Result<MovimientosPagedDto>>;

public sealed class GetMovimientosQueryValidator : AbstractValidator<GetMovimientosQuery>
{
    public GetMovimientosQueryValidator()
    {
        RuleFor(x => x.CuentaId).NotEmpty();
        RuleFor(x => x.Pagina).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Tamano).InclusiveBetween(1, 100);
    }
}

public sealed class GetMovimientosQueryHandler(
    ICuentaRepository cuentaRepository,
    ITransaccionRepository transaccionRepository)
    : IRequestHandler<GetMovimientosQuery, Result<MovimientosPagedDto>>
{
    public async Task<Result<MovimientosPagedDto>> Handle(
        GetMovimientosQuery request,
        CancellationToken cancellationToken)
    {
        var cuenta = await cuentaRepository.GetByIdAsync(request.CuentaId, cancellationToken);

        if (cuenta is null)
            return Result<MovimientosPagedDto>.Failure($"La cuenta con Id '{request.CuentaId}' no fue encontrada.");

        var transacciones = await transaccionRepository.GetByCuentaIdAsync(
            request.CuentaId, request.Pagina, request.Tamano, cancellationToken);

        var movimientos = transacciones
            .Select(t => new MovimientoDto(
                t.Id,
                t.Tipo.ToString(),
                t.Monto,
                t.SaldoResultante,
                t.CiudadNombre,
                t.EsFueraDeCiudadOrigen,
                t.Fecha))
            .ToList();

        return Result<MovimientosPagedDto>.Success(
            new MovimientosPagedDto(cuenta.Id, movimientos, movimientos.Count, request.Pagina, request.Tamano));
    }
}
