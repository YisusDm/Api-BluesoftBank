namespace BluesoftBank.Application.Cuentas.Queries;

public sealed record SaldoDto(
    Guid CuentaId,
    string NumeroCuenta,
    decimal Saldo,
    string TipoCuenta,
    DateTime ConsultadoEn);

public sealed record MovimientoDto(
    Guid Id,
    string Tipo,
    decimal Monto,
    decimal SaldoResultante,
    string Ciudad,
    bool EsFueraDeCiudadOrigen,
    DateTime Fecha);

public sealed record MovimientosPagedDto(
    Guid CuentaId,
    IReadOnlyList<MovimientoDto> Movimientos,
    int TotalRegistros,
    int Pagina,
    int Tamano);

public sealed record ExtractoMensualDto(
    Guid CuentaId,
    string NumeroCuenta,
    string Periodo,
    decimal SaldoInicial,
    decimal SaldoFinal,
    decimal TotalConsignaciones,
    decimal TotalRetiros,
    IReadOnlyList<MovimientoDto> Movimientos);

public sealed record ClienteResumenDto(
    Guid ClienteId,
    string Nombre,
    string Correo,
    string Ciudad);

public sealed record CuentaListItemDto(
    Guid CuentaId,
    string NumeroCuenta,
    string TipoCuenta,
    decimal Saldo,
    DateTime FechaCreacion,
    ClienteResumenDto Cliente);

public sealed record CuentasPagedDto(
    IReadOnlyList<CuentaListItemDto> Cuentas,
    int TotalRegistros,
    int Pagina,
    int Tamano);
