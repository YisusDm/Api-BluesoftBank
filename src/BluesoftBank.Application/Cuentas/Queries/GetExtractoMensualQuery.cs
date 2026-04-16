using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Enums;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Queries;

public sealed record GetExtractoMensualQuery(
    Guid CuentaId,
    int Mes,
    int Anio) : IRequest<Result<ExtractoMensualDto>>;

public sealed class GetExtractoMensualQueryValidator : AbstractValidator<GetExtractoMensualQuery>
{
    public GetExtractoMensualQueryValidator()
    {
        RuleFor(x => x.CuentaId).NotEmpty();
        RuleFor(x => x.Mes).InclusiveBetween(1, 12);
        RuleFor(x => x.Anio).GreaterThanOrEqualTo(2020);
    }
}

public sealed class GetExtractoMensualQueryHandler(
    ICuentaRepository cuentaRepository,
    ITransaccionRepository transaccionRepository)
    : IRequestHandler<GetExtractoMensualQuery, Result<ExtractoMensualDto>>
{
    public async Task<Result<ExtractoMensualDto>> Handle(
        GetExtractoMensualQuery request,
        CancellationToken cancellationToken)
    {
        var cuenta = await cuentaRepository.GetByIdAsync(request.CuentaId, cancellationToken);

        if (cuenta is null)
            return Result<ExtractoMensualDto>.Failure($"La cuenta con Id '{request.CuentaId}' no fue encontrada.");

        var transacciones = await transaccionRepository.GetByCuentaIdYPeriodoAsync(
            request.CuentaId, request.Mes, request.Anio, cancellationToken);

        var movimientos = transacciones
            .Select(t => new MovimientoDto(
                t.Id,
                t.Tipo.ToString(),
                t.Monto,
                t.SaldoResultante,
                t.Ciudad.Nombre,
                t.EsFueraDeCiudadOrigen,
                t.Fecha))
            .ToList();

        var totalConsignaciones = transacciones
            .Where(t => t.Tipo == TipoTransaccion.Consignacion)
            .Sum(t => t.Monto);

        var totalRetiros = transacciones
            .Where(t => t.Tipo == TipoTransaccion.Retiro)
            .Sum(t => t.Monto);

        // El saldo inicial del período es el saldo resultante de la última transacción
        // anterior al período. Si no hay transacciones previas, el saldo inicial es 0.
        // Cuando hay transacciones en el período, se deduce a partir de la primera de ellas
        // para evitar una consulta adicional a la base de datos.
        decimal saldoInicial;
        var primeraTransaccion = transacciones.FirstOrDefault(); // ya vienen ordenadas por Fecha asc
        if (primeraTransaccion is not null)
        {
            saldoInicial = primeraTransaccion.SaldoResultante
                - (primeraTransaccion.Tipo == TipoTransaccion.Consignacion
                    ? primeraTransaccion.Monto
                    : -primeraTransaccion.Monto);
        }
        else
        {
            // Sin movimientos en el período: consultamos la última transacción anterior.
            saldoInicial = await transaccionRepository.GetSaldoAntesDePeriodoAsync(
                request.CuentaId, request.Mes, request.Anio, cancellationToken) ?? 0m;
        }

        var nombreMes = new DateTime(request.Anio, request.Mes, 1).ToString("MMMM yyyy");

        return Result<ExtractoMensualDto>.Success(new ExtractoMensualDto(
            cuenta.Id,
            cuenta.NumeroCuenta,
            char.ToUpper(nombreMes[0]) + nombreMes[1..],
            saldoInicial,
            cuenta.Saldo,
            totalConsignaciones,
            totalRetiros,
            movimientos));
    }
}
