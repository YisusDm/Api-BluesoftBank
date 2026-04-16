using System.Data;
using BluesoftBank.Application.Common.Interfaces;
using BluesoftBank.Application.Cuentas.Commands;
using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BluesoftBank.Application.Tests.Handlers;

public sealed class RetirarHandlerTests
{
    private readonly Mock<ICuentaRepository> _cuentaRepo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private RetirarCommandHandler CrearHandler() =>
        new(_cuentaRepo.Object, _unitOfWork.Object);

    private static CuentaAhorro CrearCuentaConSaldo(decimal saldo)
    {
        var cuenta = new CuentaAhorro("001-TEST", new Ciudad("BOGOTA"), Guid.NewGuid());
        cuenta.Consignar(new Dinero(saldo));
        cuenta.ClearDomainEvents();
        return cuenta;
    }

    [Fact]
    public async Task Handle_CuentaConSaldoSuficiente_RetornaResultadoExitoso()
    {
        var cuenta = CrearCuentaConSaldo(3_000_000);
        _cuentaRepo.Setup(r => r.GetByIdForUpdateAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        var result = await CrearHandler().Handle(
            new RetirarCommand(cuenta.Id, 1_000_000, "BOGOTA"), default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NuevoSaldo.Should().Be(2_000_000);
    }

    [Fact]
    public async Task Handle_CuentaNoExiste_RetornaResultadoFallido()
    {
        _cuentaRepo.Setup(r => r.GetByIdForUpdateAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((CuentaAhorro?)null);

        var result = await CrearHandler().Handle(
            new RetirarCommand(Guid.NewGuid(), 1_000_000, "BOGOTA"), default);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_SaldoInsuficiente_RetornaResultadoFallido()
    {
        var cuenta = CrearCuentaConSaldo(1_500_000);
        _cuentaRepo.Setup(r => r.GetByIdForUpdateAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        var result = await CrearHandler().Handle(
            new RetirarCommand(cuenta.Id, 2_000_000, "BOGOTA"), default);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("insuficiente");
    }

    [Fact]
    public async Task Handle_Exitoso_AbreTransaccionSerializable()
    {
        var cuenta = CrearCuentaConSaldo(1_000_000);
        _cuentaRepo.Setup(r => r.GetByIdForUpdateAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        await CrearHandler().Handle(new RetirarCommand(cuenta.Id, 1_000_000, "BOGOTA"), default);

        _unitOfWork.Verify(u =>
            u.BeginTransactionAsync(IsolationLevel.Serializable, default), Times.Once);
    }

    [Fact]
    public async Task Handle_Exitoso_EfectuaCommit()
    {
        var cuenta = CrearCuentaConSaldo(1_000_000);
        _cuentaRepo.Setup(r => r.GetByIdForUpdateAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        await CrearHandler().Handle(new RetirarCommand(cuenta.Id, 1_000_000, "BOGOTA"), default);

        _unitOfWork.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_SaldoInsuficiente_EfectuaRollback()
    {
        var cuenta = CrearCuentaConSaldo(100_000);
        _cuentaRepo.Setup(r => r.GetByIdForUpdateAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        await CrearHandler().Handle(new RetirarCommand(cuenta.Id, 999_999, "BOGOTA"), default);

        _unitOfWork.Verify(u => u.RollbackAsync(default), Times.Once);
    }

    [Fact]
    public async Task Handle_RetiroFueraDeCiudad_IndicaEsFuera()
    {
        var cuenta = CrearCuentaConSaldo(1_000_000);
        _cuentaRepo.Setup(r => r.GetByIdForUpdateAsync(cuenta.Id, default)).ReturnsAsync(cuenta);

        var result = await CrearHandler().Handle(
            new RetirarCommand(cuenta.Id, 200_000, "MEDELLIN"), default);

        result.IsSuccess.Should().BeTrue();
        result.Value!.EsFueraDeCiudadOrigen.Should().BeTrue();
    }
}
