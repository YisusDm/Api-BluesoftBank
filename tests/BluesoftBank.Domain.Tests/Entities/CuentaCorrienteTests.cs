using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;
using FluentAssertions;

namespace BluesoftBank.Domain.Tests.Entities;

public sealed class CuentaCorrienteTests
{
    private static CuentaCorriente CrearCuenta(decimal saldoInicial = 0, decimal cupoSobregiro = 200_000)
    {
        var cuenta = new CuentaCorriente("002-654321", new Ciudad("CALI"), Guid.NewGuid(), cupoSobregiro);
        if (saldoInicial > 0)
            cuenta.Consignar(new Dinero(saldoInicial));
        cuenta.ClearDomainEvents();
        return cuenta;
    }

    [Fact]
    public void Retirar_DentroDelSobregiro_Permitido()
    {
        var cuenta = CrearCuenta(saldoInicial: 0, cupoSobregiro: 200_000);
        cuenta.Retirar(new Dinero(150_000), new Ciudad("CALI"));
        cuenta.Saldo.Should().Be(-150_000);
    }

    [Fact]
    public void Retirar_SuperaSobregiro_LanzaSaldoInsuficienteException()
    {
        var cuenta = CrearCuenta(saldoInicial: 0, cupoSobregiro: 200_000);
        var act = () => cuenta.Retirar(new Dinero(250_000), new Ciudad("CALI"));
        act.Should().Throw<SaldoInsuficienteException>();
    }

    [Fact]
    public void Retirar_ExactamenteAlLimiteSobregiro_Permitido()
    {
        var cuenta = CrearCuenta(saldoInicial: 0, cupoSobregiro: 200_000);
        cuenta.Retirar(new Dinero(200_000), new Ciudad("CALI"));
        cuenta.Saldo.Should().Be(-200_000);
    }

    [Fact]
    public void Retirar_ConSaldoPositivoMasSobregiro_Calcula()
    {
        var cuenta = CrearCuenta(saldoInicial: 50_000, cupoSobregiro: 100_000);
        cuenta.Retirar(new Dinero(140_000), new Ciudad("CALI"));
        cuenta.Saldo.Should().Be(-90_000);
    }

    [Fact]
    public void Retirar_SuperaLimiteTotalSaldoMasSobregiro_LanzaException()
    {
        var cuenta = CrearCuenta(saldoInicial: 50_000, cupoSobregiro: 100_000);
        var act = () => cuenta.Retirar(new Dinero(160_000), new Ciudad("CALI"));
        act.Should().Throw<SaldoInsuficienteException>();
    }

    [Fact]
    public void Constructor_CupoSobreiroNegativo_LanzaArgumentException()
    {
        var act = () => new CuentaCorriente("002-111", new Ciudad("CALI"), Guid.NewGuid(), -500);
        act.Should().Throw<ArgumentException>();
    }
}
