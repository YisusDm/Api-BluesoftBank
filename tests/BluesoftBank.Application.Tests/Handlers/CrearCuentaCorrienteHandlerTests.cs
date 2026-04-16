using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Cuentas.Commands;
using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BluesoftBank.Application.Tests.Handlers;

public sealed class CrearCuentaCorrienteHandlerTests
{
    private readonly Mock<IClienteRepository> _clienteRepo = new();
    private readonly Mock<ICuentaRepository> _cuentaRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CrearCuentaCorrienteCommandHandler CrearHandler() =>
        new(_clienteRepo.Object, _cuentaRepo.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_EmpresaNoExiste_CreaEmpresaYCuenta()
    {
        _clienteRepo.Setup(r => r.GetEmpresaByNitAsync("900123456-1", default))
            .ReturnsAsync((Empresa?)null);

        var result = await CrearHandler().Handle(
            new CrearCuentaCorrienteCommand(
                "002-TEST",
                "MEDELLIN",
                "Empresa XYZ",
                "contacto@xyz.com",
                "MEDELLIN",
                "900123456-1",
                5_000_000),
            default);

        result.IsSuccess.Should().BeTrue();
        _clienteRepo.Verify(r => r.AddAsync(It.IsAny<Cliente>(), default), Times.Once);
        _cuentaRepo.Verify(r => r.AddAsync(It.IsAny<CuentaCorriente>(), default), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_EmpresaExiste_ReutilizaEmpresa()
    {
        var empresa = new Empresa("Empresa XYZ", "contacto@xyz.com", new Ciudad("MEDELLIN"), "900123456-1");
        _clienteRepo.Setup(r => r.GetEmpresaByNitAsync("900123456-1", default))
            .ReturnsAsync(empresa);

        var result = await CrearHandler().Handle(
            new CrearCuentaCorrienteCommand(
                "002-TEST",
                "MEDELLIN",
                "Empresa XYZ",
                "contacto@xyz.com",
                "MEDELLIN",
                "900123456-1",
                5_000_000),
            default);

        result.IsSuccess.Should().BeTrue();
        _clienteRepo.Verify(r => r.AddAsync(It.IsAny<Cliente>(), default), Times.Never);
    }

    [Fact]
    public async Task Handle_Exitoso_DevuelveCuentaCreada()
    {
        var empresa = new Empresa("Empresa XYZ", "contacto@xyz.com", new Ciudad("MEDELLIN"), "900123456-1");
        _clienteRepo.Setup(r => r.GetEmpresaByNitAsync("900123456-1", default))
            .ReturnsAsync(empresa);

        var result = await CrearHandler().Handle(
            new CrearCuentaCorrienteCommand(
                "002-TEST",
                "MEDELLIN",
                "Empresa XYZ",
                "contacto@xyz.com",
                "MEDELLIN",
                "900123456-1",
                5_000_000),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NumeroCuenta.Should().Be("002-TEST");
        result.Value!.ClienteId.Should().Be(empresa.Id);
    }
}
