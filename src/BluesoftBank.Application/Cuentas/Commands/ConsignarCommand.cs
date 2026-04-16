using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;
using FluentValidation;
using MediatR;

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
    IUnitOfWork unitOfWork)
    : IRequestHandler<ConsignarCommand, Result<ConsignarResponse>>
{
    public async Task<Result<ConsignarResponse>> Handle(
        ConsignarCommand request,
        CancellationToken cancellationToken)
    {
        // Sin UPDLOCK: las consignaciones solo suman saldo y no pueden producir saldo negativo,
        // por lo que la lectura optimista es suficiente y se evita contención innecesaria.
        var cuenta = await cuentaRepository.GetByIdAsync(request.CuentaId, cancellationToken);

        if (cuenta is null)
            return Result<ConsignarResponse>.Failure($"La cuenta con Id '{request.CuentaId}' no fue encontrada.");

        try
        {
            var monto = new Dinero(request.Monto);
            cuenta.Consignar(monto);
        }
        catch (DomainException ex)
        {
            return Result<ConsignarResponse>.Failure(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var ultima = cuenta.Transacciones.OrderByDescending(t => t.Fecha).First();
        return Result<ConsignarResponse>.Success(
            new ConsignarResponse(cuenta.Id, cuenta.Saldo, ultima.Id, ultima.Fecha));
    }
}
