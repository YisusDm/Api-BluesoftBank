using BluesoftBank.Domain.ValueObjects;

namespace BluesoftBank.Domain.Entities;

public sealed class PersonaNatural : Cliente
{
    private PersonaNatural() { }

    public PersonaNatural(string nombre, string correo, Ciudad ciudad, string cedula)
        : base(nombre, correo, ciudad)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(cedula);
        Cedula = cedula;
    }

    public string Cedula { get; private set; } = null!;
}
