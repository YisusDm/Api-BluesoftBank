using BluesoftBank.Domain.ValueObjects;

namespace BluesoftBank.Domain.Entities;

public abstract class Cliente : Entity
{
    protected Cliente() { }

    protected Cliente(string nombre, string correo, Ciudad ciudad)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        ArgumentException.ThrowIfNullOrWhiteSpace(correo);
        ArgumentNullException.ThrowIfNull(ciudad);

        Nombre = nombre;
        Correo = correo;
        Ciudad = ciudad;
    }

    public string Nombre { get; private set; } = null!;
    public string Correo { get; private set; } = null!;
    public Ciudad Ciudad { get; private set; } = null!;
}
