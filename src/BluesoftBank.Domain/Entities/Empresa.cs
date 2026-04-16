using BluesoftBank.Domain.ValueObjects;

namespace BluesoftBank.Domain.Entities;

public sealed class Empresa : Cliente
{
    private Empresa() { }

    public Empresa(string nombre, string correo, Ciudad ciudad, string nit)
        : base(nombre, correo, ciudad)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nit);
        Nit = nit;
    }

    public string Nit { get; private set; } = null!;
}
