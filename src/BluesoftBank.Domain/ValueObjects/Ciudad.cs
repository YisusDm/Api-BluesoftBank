namespace BluesoftBank.Domain.ValueObjects;

public sealed record Ciudad
{
    public string Nombre { get; }

    public Ciudad(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la ciudad no puede estar vacío.", nameof(nombre));

        Nombre = nombre.Trim().ToUpperInvariant();
    }

    public override string ToString() => Nombre;
}
