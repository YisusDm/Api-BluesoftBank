using BluesoftBank.Domain.Exceptions;

namespace BluesoftBank.Domain.ValueObjects;

public sealed record Dinero
{
    public decimal Valor { get; }

    public Dinero(decimal valor)
    {
        if (valor <= 0)
            throw new MontoInvalidoException(valor);

        Valor = valor;
    }

    public static implicit operator decimal(Dinero dinero) => dinero.Valor;

    public override string ToString() => Valor.ToString("C");
}
