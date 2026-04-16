using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Entities;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Queries;

public sealed record GetCuentaByIdQuery(Guid CuentaId) : IRequest<Result<CuentaDetalleDto>>;

public sealed class GetCuentaByIdQueryHandler(
    ICuentaRepository cuentaRepository,
    IClienteRepository clienteRepository)
    : IRequestHandler<GetCuentaByIdQuery, Result<CuentaDetalleDto>>
{
    public async Task<Result<CuentaDetalleDto>> Handle(
        GetCuentaByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cuenta = await cuentaRepository.GetByIdAsync(request.CuentaId, cancellationToken);

        if (cuenta is null)
            return Result<CuentaDetalleDto>.Failure($"La cuenta con Id '{request.CuentaId}' no fue encontrada.");

        var cliente = await clienteRepository.GetByIdAsync(cuenta.ClienteId, cancellationToken);

        var tipoCuenta = cuenta switch
        {
            CuentaAhorro => "CuentaAhorro",
            CuentaCorriente => "CuentaCorriente",
            _ => "Desconocido"
        };

        return Result<CuentaDetalleDto>.Success(new CuentaDetalleDto(
            cuenta.Id,
            cuenta.NumeroCuenta,
            tipoCuenta,
            cuenta.Saldo,
            cuenta.Ciudad.Nombre,
            cuenta.FechaCreacion,
            cliente?.Nombre ?? string.Empty,
            cuenta.ClienteId,
            cliente?.Correo ?? string.Empty,
            cliente?.Ciudad.Nombre ?? string.Empty));
    }
}
