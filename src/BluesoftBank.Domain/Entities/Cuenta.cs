using BluesoftBank.Domain.Enums;
using BluesoftBank.Domain.Events;
using BluesoftBank.Domain.Exceptions;
using BluesoftBank.Domain.ValueObjects;

namespace BluesoftBank.Domain.Entities;

public abstract class Cuenta : Entity
{
    private readonly List<Transaccion> _transacciones = [];

    protected Cuenta() { }

    protected Cuenta(string numeroCuenta, Ciudad ciudad, Guid clienteId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(numeroCuenta);
        ArgumentNullException.ThrowIfNull(ciudad);
        if (clienteId == Guid.Empty) throw new ArgumentException("ClienteId no puede ser vacío.", nameof(clienteId));

        NumeroCuenta = numeroCuenta;
        Ciudad = ciudad;
        ClienteId = clienteId;
        Saldo = 0m;
        FechaCreacion = DateTime.UtcNow;
    }

    public string NumeroCuenta { get; private set; } = null!;
    public decimal Saldo { get; private set; }
    public Ciudad Ciudad { get; private set; } = null!;
    public Guid ClienteId { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public IReadOnlyCollection<Transaccion> Transacciones => _transacciones.AsReadOnly();

    public void Consignar(Dinero monto)
    {
        ArgumentNullException.ThrowIfNull(monto);

        Saldo += monto.Valor;

        var transaccion = new Transaccion(Id, TipoTransaccion.Consignacion, monto.Valor, Saldo, Ciudad, false);
        _transacciones.Add(transaccion);

        AddDomainEvent(new ConsignacionRegistradaEvent(Id, monto.Valor, Saldo, DateTime.UtcNow));
    }

    public void Retirar(Dinero monto, Ciudad ciudadRetiro)
    {
        ArgumentNullException.ThrowIfNull(monto);
        ArgumentNullException.ThrowIfNull(ciudadRetiro);

        if (monto.Valor < 1_000_000)
            throw new MontoMinimoRetiroException(monto.Valor, 1_000_000);

        ValidarRetiro(monto);

        bool esFuera = ciudadRetiro.Nombre != Ciudad.Nombre;
        Saldo -= monto.Valor;

        var transaccion = new Transaccion(Id, TipoTransaccion.Retiro, monto.Valor, Saldo, ciudadRetiro, esFuera);
        _transacciones.Add(transaccion);

        AddDomainEvent(new RetiroRegistradoEvent(Id, monto.Valor, Saldo, ciudadRetiro.Nombre, esFuera, DateTime.UtcNow));
    }

    // Template Method: cada subclase define su regla de validación
    protected abstract void ValidarRetiro(Dinero monto);
}
