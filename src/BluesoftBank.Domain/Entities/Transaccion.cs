using BluesoftBank.Domain.Enums;
using BluesoftBank.Domain.ValueObjects;

namespace BluesoftBank.Domain.Entities;

public sealed class Transaccion : Entity
{
    private Transaccion() { }

    internal Transaccion(
        Guid cuentaId,
        TipoTransaccion tipo,
        decimal monto,
        decimal saldoResultante,
        Ciudad ciudad,
        bool esFueraDeCiudadOrigen)
    {
        CuentaId = cuentaId;
        Tipo = tipo;
        Monto = monto;
        SaldoResultante = saldoResultante;
        Ciudad = ciudad;
        EsFueraDeCiudadOrigen = esFueraDeCiudadOrigen;
        Fecha = DateTime.UtcNow;
    }

    public Guid CuentaId { get; private set; }
    public TipoTransaccion Tipo { get; private set; }
    public decimal Monto { get; private set; }
    public decimal SaldoResultante { get; private set; }
    public Ciudad Ciudad { get; private set; } = null!;
    public bool EsFueraDeCiudadOrigen { get; private set; }
    public DateTime Fecha { get; private set; }
}
