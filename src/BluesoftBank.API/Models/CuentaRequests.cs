namespace BluesoftBank.API.Models;

public sealed record ConsignarRequest(decimal Monto, string Ciudad);
public sealed record RetirarRequest(decimal Monto, string Ciudad);

public sealed record CrearCuentaAhorroRequest(
    string NumeroCuenta,
    string Ciudad,
    string Nombre,
    string Correo,
    string CiudadCliente,
    string Cedula);

public sealed record CrearCuentaCorrienteRequest(
    string NumeroCuenta,
    string Ciudad,
    string Nombre,
    string Correo,
    string CiudadCliente,
    string Nit,
    decimal CupoSobregiro);
