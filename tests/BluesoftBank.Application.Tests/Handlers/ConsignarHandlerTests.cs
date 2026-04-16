using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Cuentas.Commands;
using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BluesoftBank.Application.Tests.Handlers;

public sealed class ConsignarHandlerTests
{
    private readonly Mock<ICuentaRepository> _cuentaRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private ConsignarCommandHandler CrearHandler() =>
        new(_cuentaRepo.Object, _unitOfWork.Object);

    private static CuentaAhorro CrearCuentaConSaldo(decimal saldo = 0)
    {
        var cuenta = new CuentaAhorro("001-TEST", new Ciudad("BOGOTA"), Guid.NewGuid());
        if (saldo > 0) cuenta.Consignar(new Dinero(saldo));
        cuenta.ClearDomainEvents();
        return cuenta;
    }

    [Fact]
    public async Task Handle_CuentaExiste_RetornaResultadoExitoso()
    {
        var cuenta = CrearCuentaConSaldo();
        _cuentaRepo.Setup(r => r.GetByIdAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        var result = await CrearHandler().Handle(
            new ConsignarCommand(cuenta.Id, 500_000, "BOGOTA"), default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NuevoSaldo.Should().Be(500_000);
    }

    [Fact]
    public async Task Handle_CuentaNoExiste_RetornaResultadoFallido()
    {
        _cuentaRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default)).ReturnsAsync((CuentaAhorro?)null);

        var result = await CrearHandler().Handle(
            new ConsignarCommand(Guid.NewGuid(), 500_000, "BOGOTA"), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("no fue encontrada");
    }

    [Fact]
    public async Task Handle_Exitoso_LlamaSaveChangesUnaVez()
    {
        var cuenta = CrearCuentaConSaldo();
        _cuentaRepo.Setup(r => r.GetByIdAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        await CrearHandler().Handle(new ConsignarCommand(cuenta.Id, 100_000, "BOGOTA"), default);

        _unitOfWork.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_Exitoso_TransaccionRegistrada()
    {
        var cuenta = CrearCuentaConSaldo();
        _cuentaRepo.Setup(r => r.GetByIdAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        await CrearHandler().Handle(new ConsignarCommand(cuenta.Id, 300_000, "BOGOTA"), default);

        cuenta.Transacciones.Should().HaveCount(1);
    }
}
