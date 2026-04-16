using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Commands;

public sealed record CrearCuentaAhorroCommand(
    string NumeroCuenta,
    string Ciudad,
    string Nombre,
    string Correo,
    string CiudadCliente,
    string Cedula) : IRequest<Result<CuentaCreatedResponse>>;

public sealed class CrearCuentaAhorroCommandValidator : AbstractValidator<CrearCuentaAhorroCommand>
{
    public CrearCuentaAhorroCommandValidator()
    {
        RuleFor(x => x.NumeroCuenta).NotEmpty().Length(5, 20);
        RuleFor(x => x.Ciudad).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().Length(3, 200);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress();
        RuleFor(x => x.CiudadCliente).NotEmpty();
        RuleFor(x => x.Cedula).NotEmpty().Length(6, 20);
    }
}

public sealed class CrearCuentaAhorroCommandHandler(
    IClienteRepository clienteRepository,
    ICuentaRepository cuentaRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CrearCuentaAhorroCommand, Result<CuentaCreatedResponse>>
{
    public async Task<Result<CuentaCreatedResponse>> Handle(
        CrearCuentaAhorroCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await clienteRepository.GetPersonaNaturalByCedulaAsync(
                request.Cedula, cancellationToken);

            if (cliente is null)
            {
                cliente = new PersonaNatural(
                    request.Nombre,
                    request.Correo,
                    new Ciudad(request.CiudadCliente),
                    request.Cedula);
                await clienteRepository.AddAsync(cliente, cancellationToken);
            }

            var cuenta = new CuentaAhorro(
                request.NumeroCuenta,
                new Ciudad(request.Ciudad),
                cliente.Id);

            await cuentaRepository.AddAsync(cuenta, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<CuentaCreatedResponse>.Success(
                new CuentaCreatedResponse(cuenta.Id, request.NumeroCuenta, cliente.Id, cliente.Nombre));
        }
        catch (DomainException ex)
        {
            return Result<CuentaCreatedResponse>.Failure(ex.Message);
        }
    }
}

public sealed record CuentaCreatedResponse(
    Guid CuentaId,
    string NumeroCuenta,
    Guid ClienteId,
    string NombreCliente);
