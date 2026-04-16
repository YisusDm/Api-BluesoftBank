using BluesoftBank.Domain.Entities;
using BluesoftBank.Domain.Events;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;
using FluentAssertions;

namespace BluesoftBank.Domain.Tests.Entities;

public sealed class CuentaAhorroTests
{
    private static CuentaAhorro CrearCuenta(decimal saldoInicial = 0)
    {
        var cuenta = new CuentaAhorro("001-123456", new Ciudad("BOGOTA"), Guid.NewGuid());
        if (saldoInicial > 0)
            cuenta.Consignar(new Dinero(saldoInicial));
        cuenta.ClearDomainEvents();
        return cuenta;
    }

    [Fact]
    public void Consignar_MontoValido_IncrementaSaldo()
    {
        var cuenta = CrearCuenta(1_000_000);
        cuenta.Consignar(new Dinero(500_000));
        cuenta.Saldo.Should().Be(1_500_000);
    }

    [Fact]
    public void Consignar_MontoNegativo_LanzaMontoInvalidoException()
    {
        var cuenta = CrearCuenta();
        var act = () => cuenta.Consignar(new Dinero(-100));
        act.Should().Throw<MontoInvalidoException>();
    }

    [Fact]
    public void Consignar_MontoZero_LanzaMontoInvalidoException()
    {
        var cuenta = CrearCuenta();
        var act = () => cuenta.Consignar(new Dinero(0));
        act.Should().Throw<MontoInvalidoException>();
    }

    [Fact]
    public void Retirar_MontoValido_DecrementaSaldo()
    {
        var cuenta = CrearCuenta(1_000_000);
        cuenta.Retirar(new Dinero(200_000), new Ciudad("BOGOTA"));
        cuenta.Saldo.Should().Be(800_000);
    }

    [Fact]
    public void Retirar_SaldoInsuficiente_LanzaSaldoInsuficienteException()
    {
        var cuenta = CrearCuenta(1_000_000);
        var act = () => cuenta.Retirar(new Dinero(1_500_000), new Ciudad("BOGOTA"));
        act.Should().Throw<SaldoInsuficienteException>();
    }

    [Fact]
    public void Retirar_MontoExactoAlSaldo_Permitido()
    {
        var cuenta = CrearCuenta(1_000_000);
        cuenta.Retirar(new Dinero(1_000_000), new Ciudad("BOGOTA"));
        cuenta.Saldo.Should().Be(0);
    }

    [Fact]
    public void Retirar_FueraDeCiudad_MarcaTransaccionComoFuera()
    {
        var cuenta = CrearCuenta(1_000_000);
        cuenta.Retirar(new Dinero(200_000), new Ciudad("MEDELLIN"));
        cuenta.Transacciones.Last().EsFueraDeCiudadOrigen.Should().BeTrue();
    }

    [Fact]
    public void Retirar_EnMismaCiudad_NoMarcaComoFuera()
    {
        var cuenta = CrearCuenta(1_000_000);
        cuenta.Retirar(new Dinero(200_000), new Ciudad("BOGOTA"));
        cuenta.Transacciones.Last().EsFueraDeCiudadOrigen.Should().BeFalse();
    }

    [Fact]
    public void Consignar_DispararaConsignacionRegistradaEvent()
    {
        var cuenta = CrearCuenta();
        cuenta.Consignar(new Dinero(500_000));
        cuenta.GetDomainEvents().Should().ContainSingle(e => e is ConsignacionRegistradaEvent);
    }

    [Fact]
    public void Retirar_DispararaRetiroRegistradoEvent()
    {
        var cuenta = CrearCuenta(1_000_000);
        cuenta.Retirar(new Dinero(200_000), new Ciudad("BOGOTA"));
        cuenta.GetDomainEvents().Should().ContainSingle(e => e is RetiroRegistradoEvent);
    }
}
