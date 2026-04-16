using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Cuentas.Commands;
using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BluesoftBank.Application.Tests.Handlers;

public sealed class CrearCuentaAhorroHandlerTests
{
    private readonly Mock<IClienteRepository> _clienteRepo = new();
    private readonly Mock<ICuentaRepository> _cuentaRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private CrearCuentaAhorroCommandHandler CrearHandler() =>
        new(_clienteRepo.Object, _cuentaRepo.Object, _unitOfWork.Object);

    [Fact]
    public async Task Handle_ClienteNoExiste_CreaClienteYCuenta()
    {
        _clienteRepo.Setup(r => r.GetPersonaNaturalByCedulaAsync("123456", default))
            .ReturnsAsync((PersonaNatural?)null);

        var result = await CrearHandler().Handle(
            new CrearCuentaAhorroCommand(
                "001-TEST",
                "BOGOTA",
                "Juan García",
                "juan@email.com",
                "BOGOTA",
                "123456"),
            default);

        result.IsSuccess.Should().BeTrue();
        _clienteRepo.Verify(r => r.AddAsync(It.IsAny<Cliente>(), default), Times.Once);
        _cuentaRepo.Verify(r => r.AddAsync(It.IsAny<CuentaAhorro>(), default), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_ClienteExiste_ReutilizaCliente()
    {
        var cliente = new PersonaNatural("Juan García", "juan@email.com", new Ciudad("BOGOTA"), "123456");
        _clienteRepo.Setup(r => r.GetPersonaNaturalByCedulaAsync("123456", default))
            .ReturnsAsync(cliente);

        var result = await CrearHandler().Handle(
            new CrearCuentaAhorroCommand(
                "001-TEST",
                "BOGOTA",
                "Juan García",
                "juan@email.com",
                "BOGOTA",
                "123456"),
            default);

        result.IsSuccess.Should().BeTrue();
        _clienteRepo.Verify(r => r.AddAsync(It.IsAny<Cliente>(), default), Times.Never);
        _cuentaRepo.Verify(r => r.AddAsync(It.IsAny<CuentaAhorro>(), default), Times.Once);
    }

    [Fact]
    public async Task Handle_Exitoso_DevuelveCuentaCreada()
    {
        var cliente = new PersonaNatural("Juan García", "juan@email.com", new Ciudad("BOGOTA"), "123456");
        _clienteRepo.Setup(r => r.GetPersonaNaturalByCedulaAsync("123456", default))
            .ReturnsAsync(cliente);

        var result = await CrearHandler().Handle(
            new CrearCuentaAhorroCommand(
                "001-TEST",
                "BOGOTA",
                "Juan García",
                "juan@email.com",
                "BOGOTA",
                "123456"),
            default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NumeroCuenta.Should().Be("001-TEST");
        result.Value!.ClienteId.Should().Be(cliente.Id);
    }
}
