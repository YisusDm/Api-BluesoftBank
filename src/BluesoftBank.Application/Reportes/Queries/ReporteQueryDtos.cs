namespace BluesoftBank.Application.Reportes.Queries;

public sealed record TopClienteDto(
    Guid ClienteId,
    string Nombre,
    int TotalTransacciones,
    int TotalConsignaciones,
    int TotalRetiros);

public sealed record RetiroFueraCiudadDto(
    Guid ClienteId,
    string Nombre,
    string CiudadOrigen,
    int TotalRetirosFueraCiudad,
    decimal ValorTotalRetiros,
    DateTime UltimoRetiro);
