using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Common.Results;
using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace BluesoftBank.Application.Cuentas.Commands;

public sealed record CrearCuentaCorrienteCommand(
    string NumeroCuenta,
    string Ciudad,
    string Nombre,
    string Correo,
    string CiudadCliente,
    string Nit,
    decimal CupoSobregiro) : IRequest<Result<CuentaCreatedResponse>>;

public sealed class CrearCuentaCorrienteCommandValidator : AbstractValidator<CrearCuentaCorrienteCommand>
{
    public CrearCuentaCorrienteCommandValidator()
    {
        RuleFor(x => x.NumeroCuenta).NotEmpty().Length(5, 20);
        RuleFor(x => x.Ciudad).NotEmpty();
        RuleFor(x => x.Nombre).NotEmpty().Length(3, 200);
        RuleFor(x => x.Correo).NotEmpty().EmailAddress();
        RuleFor(x => x.CiudadCliente).NotEmpty();
        RuleFor(x => x.Nit).NotEmpty().Length(6, 20);
        RuleFor(x => x.CupoSobregiro).GreaterThan(0).WithMessage("El cupo debe ser mayor a cero.");
    }
}

public sealed class CrearCuentaCorrienteCommandHandler(
    IClienteRepository clienteRepository,
    ICuentaRepository cuentaRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<CrearCuentaCorrienteCommand, Result<CuentaCreatedResponse>>
{
    public async Task<Result<CuentaCreatedResponse>> Handle(
        CrearCuentaCorrienteCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await clienteRepository.GetEmpresaByNitAsync(
                request.Nit, cancellationToken);

            if (cliente is null)
            {
                cliente = new Empresa(
                    request.Nombre,
                    request.Correo,
                    new Ciudad(request.CiudadCliente),
                    request.Nit);
                await clienteRepository.AddAsync(cliente, cancellationToken);
            }

            var cuenta = new CuentaCorriente(
                request.NumeroCuenta,
                new Ciudad(request.Ciudad),
                cliente.Id,
                request.CupoSobregiro);

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
