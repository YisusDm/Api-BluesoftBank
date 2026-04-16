using BluesoftBank.Domain.Interfaces;

namespace BluesoftBank.Domain.Events;

public sealed record ConsignacionRegistradaEvent(
    Guid CuentaId,
    decimal Monto,
    decimal NuevoSaldo,
    DateTime Fecha) : IDomainEvent;

public sealed record RetiroRegistradoEvent(
    Guid CuentaId,
    decimal Monto,
    decimal NuevoSaldo,
    string Ciudad,
    bool EsFueraDeCiudadOrigen,
    DateTime Fecha) : IDomainEvent;
