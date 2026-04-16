using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BluesoftBank.Application.Cuentas.Commands;

public sealed record ConsignarCommand(
    Guid CuentaId,
    decimal Monto,
    string Ciudad) : IRequest<Result<ConsignarResponse>>;

public sealed class ConsignarCommandValidator : AbstractValidator<ConsignarCommand>
{
    public ConsignarCommandValidator()
    {
        RuleFor(x => x.CuentaId).NotEmpty();
        RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
        RuleFor(x => x.Ciudad).NotEmpty().WithMessage("La ciudad es requerida.");
    }
}

public sealed class ConsignarCommandHandler(
    ICuentaRepository cuentaRepository,
    ITransaccionRepository transaccionRepository,
    IUnitOfWork unitOfWork,
    ILogger<ConsignarCommandHandler> logger)
    : IRequestHandler<ConsignarCommand, Result<ConsignarResponse>>
{
    public async Task<Result<ConsignarResponse>> Handle(
        ConsignarCommand request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("📥 [CONSIGNAR INICIO] CuentaId: {CuentaId}, Monto: {Monto}, Ciudad: {Ciudad}",
            request.CuentaId, request.Monto, request.Ciudad);

        try
        {
            // PASO 1: Cargar cuenta (sin lock: las consignaciones no pueden causar saldo negativo)
            logger.LogInformation("🔍 [PASO 1] Cargando cuenta...");
            var cuenta = await cuentaRepository.GetByIdAsync(request.CuentaId, cancellationToken);
            logger.LogInformation("✅ [PASO 1 OK] Cuenta cargada. Existe: {Existe}", cuenta is not null);

            if (cuenta is null)
            {
                logger.LogWarning("❌ [PASO 1 FALLO] Cuenta no encontrada: {CuentaId}", request.CuentaId);
                return Result<ConsignarResponse>.Failure($"La cuenta con Id '{request.CuentaId}' no fue encontrada.");
            }

            logger.LogInformation("📊 [PASO 1 INFO] Saldo actual: {Saldo}, Cliente: {ClienteId}, Transacciones previas: {TransaccionCount}",
                cuenta.Saldo, cuenta.ClienteId, cuenta.Transacciones.Count);

            // PASO 2: Validar y crear Dinero
            logger.LogInformation("🔍 [PASO 2] Validando monto...");
            var monto = new Dinero(request.Monto);
            var ciudad = new Ciudad(request.Ciudad);
            logger.LogInformation("✅ [PASO 2 OK] Monto y ciudad validados");

            // PASO 3: Aplicar lógica de dominio
            logger.LogInformation("🔍 [PASO 3] Aplicando consignación en dominio...");
            cuenta.Consignar(monto);
            logger.LogInformation("✅ [PASO 3 OK] Consignación aplicada. Nuevo saldo: {NuevoSaldo}, Total transacciones: {TransaccionCount}",
                cuenta.Saldo, cuenta.Transacciones.Count);

            // PASO 3.5: Agregar transacción manualmente al repositorio
            logger.LogInformation("🔍 [PASO 3.5] Registrando transacción en el repositorio...");
            var ultimaTransaccion = cuenta.Transacciones.OrderByDescending(t => t.Fecha).First();
            await transaccionRepository.AddAsync(ultimaTransaccion, cancellationToken);
            logger.LogInformation("✅ [PASO 3.5 OK] Transacción registrada");

            // PASO 4: Guardar cambios
            logger.LogInformation("🔍 [PASO 4] Guardando cambios en base de datos...");
            await unitOfWork.SaveChangesAsync(cancellationToken);
            logger.LogInformation("✅ [PASO 4 OK] Cambios guardados exitosamente");

            // PASO 5: Obtener última transacción
            logger.LogInformation("🔍 [PASO 5] Obteniendo última transacción...");
            var resultado = cuenta.Transacciones.OrderByDescending(t => t.Fecha).First();
            logger.LogInformation("✅ [PASO 5 OK] Transacción obtenida. Id: {TransaccionId}, Monto: {Monto}, Fecha: {Fecha}",
                resultado.Id, resultado.Monto, resultado.Fecha);

            logger.LogInformation("✅ [CONSIGNAR EXITOSO] Saldo final: {SaldoFinal}", cuenta.Saldo);
            return Result<ConsignarResponse>.Success(
                new ConsignarResponse(cuenta.Id, cuenta.Saldo, resultado.Id, resultado.Fecha));
        }
        catch (DomainException ex)
        {
            logger.LogError("❌ [CONSIGNAR FALLO - DOMAIN] {Mensaje}", ex.Message);
            return Result<ConsignarResponse>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ [CONSIGNAR FALLO - EXCEPCIÓN] Tipo: {ExceptionType}, Mensaje: {Mensaje}",
                ex.GetType().Name, ex.Message);

            if (ex.GetType().Name == "DbUpdateConcurrencyException")
            {
                logger.LogError("🔴 [CONCURRENCIA] Conflicto de concurrencia detectado. Intentando nuevamente...");
                return Result<ConsignarResponse>.Failure(
                    $"No fue posible completar la consignación por conflicto de concurrencia. Intente nuevamente.");
            }

            throw;
        }
    }
}
