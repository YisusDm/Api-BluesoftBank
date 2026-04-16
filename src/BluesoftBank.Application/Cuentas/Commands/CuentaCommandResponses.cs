namespace BluesoftBank.Application.Cuentas.Commands;

public sealed record ConsignarResponse(
    Guid CuentaId,
    decimal NuevoSaldo,
    Guid TransaccionId,
    DateTime Fecha);

public sealed record RetirarResponse(
    Guid CuentaId,
    decimal NuevoSaldo,
    Guid TransaccionId,
    bool EsFueraDeCiudadOrigen,
    DateTime Fecha);
