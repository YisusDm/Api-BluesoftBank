using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Entities;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Queries;

public sealed record GetSaldoQuery(Guid CuentaId) : IRequest<Result<SaldoDto>>;

public sealed class GetSaldoQueryHandler(ICuentaRepository cuentaRepository)
    : IRequestHandler<GetSaldoQuery, Result<SaldoDto>>
{
    public async Task<Result<SaldoDto>> Handle(GetSaldoQuery request, CancellationToken cancellationToken)
    {
        var cuenta = await cuentaRepository.GetByIdAsync(request.CuentaId, cancellationToken);

        if (cuenta is null)
            return Result<SaldoDto>.Failure($"La cuenta con Id '{request.CuentaId}' no fue encontrada.");

        var tipo = cuenta switch
        {
            CuentaAhorro => "CuentaAhorro",
            CuentaCorriente => "CuentaCorriente",
            _ => "Desconocido"
        };

        return Result<SaldoDto>.Success(
            new SaldoDto(cuenta.Id, cuenta.NumeroCuenta, cuenta.Saldo, tipo, DateTime.UtcNow));
    }
}
