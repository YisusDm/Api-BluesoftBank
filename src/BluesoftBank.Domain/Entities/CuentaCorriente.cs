using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;

namespace BluesoftBank.Domain.Entities;

public sealed class CuentaCorriente : Cuenta
{
    private CuentaCorriente() { }

    public CuentaCorriente(string numeroCuenta, Ciudad ciudad, Guid clienteId, decimal cupoSobregiro)
        : base(numeroCuenta, ciudad, clienteId)
    {
        if (cupoSobregiro < 0)
            throw new ArgumentException("El cupo de sobregiro no puede ser negativo.", nameof(cupoSobregiro));

        CupoSobregiro = cupoSobregiro;
    }

    public decimal CupoSobregiro { get; private set; }

    protected override void ValidarRetiro(Dinero monto)
    {
        if (Saldo - monto.Valor < -CupoSobregiro)
            throw new SaldoInsuficienteException(Saldo + CupoSobregiro, monto.Valor);
    }
}
