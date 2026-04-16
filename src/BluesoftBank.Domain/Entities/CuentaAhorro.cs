using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;

namespace BluesoftBank.Domain.Entities;

public sealed class CuentaAhorro : Cuenta
{
    private CuentaAhorro() { }

    public CuentaAhorro(string numeroCuenta, Ciudad ciudad, Guid clienteId)
        : base(numeroCuenta, ciudad, clienteId) { }

    protected override void ValidarRetiro(Dinero monto)
    {
        if (Saldo - monto.Valor < 0)
            throw new SaldoInsuficienteException(Saldo, monto.Valor);
    }
}
