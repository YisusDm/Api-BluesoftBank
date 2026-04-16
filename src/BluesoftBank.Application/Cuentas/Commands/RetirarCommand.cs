using System.Data;
using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Commands;

public sealed record RetirarCommand(
    Guid CuentaId,
    decimal Monto,
    string Ciudad) : IRequest<Result<RetirarResponse>>;

public sealed class RetirarCommandValidator : AbstractValidator<RetirarCommand>
{
    public RetirarCommandValidator()
    {
        RuleFor(x => x.CuentaId).NotEmpty();
        RuleFor(x => x.Monto).GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");
        RuleFor(x => x.Ciudad).NotEmpty().WithMessage("La ciudad es requerida.");
    }
}

public sealed class RetirarCommandHandler(
    ICuentaRepository cuentaRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RetirarCommand, Result<RetirarResponse>>
{
    public async Task<Result<RetirarResponse>> Handle(
        RetirarCommand request,
        CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            // UPDLOCK garantiza que lecturas concurrentes sobre la misma cuenta esperen
            var cuenta = await cuentaRepository.GetByIdForUpdateAsync(request.CuentaId, cancellationToken);

            if (cuenta is null)
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                return Result<RetirarResponse>.Failure($"La cuenta con Id '{request.CuentaId}' no fue encontrada.");
            }

            var monto = new Dinero(request.Monto);
            var ciudad = new Ciudad(request.Ciudad);

            cuenta.Retirar(monto, ciudad);

            await unitOfWork.CommitAsync(cancellationToken);

            var ultima = cuenta.Transacciones.OrderByDescending(t => t.Fecha).First();
            return Result<RetirarResponse>.Success(
                new RetirarResponse(cuenta.Id, cuenta.Saldo, ultima.Id, ultima.EsFueraDeCiudadOrigen, ultima.Fecha));
        }
        catch (DomainException ex)
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            return Result<RetirarResponse>.Failure(ex.Message);
        }
    }
}
