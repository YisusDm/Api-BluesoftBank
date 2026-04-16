using BluesoftBank.API.Models;
using BluesoftBank.Application.Cuentas.Commands;
using BluesoftBank.Application.Cuentas.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BluesoftBank.API.Controllers;

[ApiController]
[Route("api/cuentas")]
[Produces("application/json")]
public sealed class CuentasController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista todas las cuentas con paginación.
    /// Retorna nombre del cliente, número de cuenta, saldo y fecha de creación.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CuentasPagedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ListarCuentas(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetCuentasQuery(pagina, tamano), ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : Ok(result.Value);
    }

    /// <summary>Crea una nueva cuenta de ahorro para una persona natural.
    /// Si el cliente (cédula) ya existe, se asocia a esa persona.
    /// Si no existe, se crea un nuevo cliente.
    /// </summary>
    [HttpPost("ahorro")]
    [ProducesResponseType(typeof(CuentaCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearCuentaAhorro(
        [FromBody] CrearCuentaAhorroRequest request,
        CancellationToken ct = default)
    {
        var command = new CrearCuentaAhorroCommand(
            request.NumeroCuenta,
            request.Ciudad,
            request.Nombre,
            request.Correo,
            request.CiudadCliente,
            request.Cedula);

        var result = await mediator.Send(command, ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : CreatedAtAction(nameof(GetSaldo), new { id = result.Value!.CuentaId }, result.Value);
    }

    /// <summary>Crea una nueva cuenta corriente para una empresa.
    /// Si el cliente (NIT) ya existe, se asocia a esa empresa.
    /// Si no existe, se crea una nueva empresa.
    /// </summary>
    [HttpPost("corriente")]
    [ProducesResponseType(typeof(CuentaCreatedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CrearCuentaCorriente(
        [FromBody] CrearCuentaCorrienteRequest request,
        CancellationToken ct = default)
    {
        var command = new CrearCuentaCorrienteCommand(
            request.NumeroCuenta,
            request.Ciudad,
            request.Nombre,
            request.Correo,
            request.CiudadCliente,
            request.Nit,
            request.CupoSobregiro);

        var result = await mediator.Send(command, ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : CreatedAtAction(nameof(GetSaldo), new { id = result.Value!.CuentaId }, result.Value);
    }

    /// <summary>Realiza una consignación en la cuenta especificada.</summary>
    [HttpPost("{id:guid}/consignar")]
    [ProducesResponseType(typeof(ConsignarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Consignar(Guid id, [FromBody] ConsignarRequest request, CancellationToken ct)
    {
        var command = new ConsignarCommand(id, request.Monto, request.Ciudad);
        var result = await mediator.Send(command, ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : Ok(result.Value);
    }

    /// <summary>Realiza un retiro de la cuenta especificada.</summary>
    [HttpPost("{id:guid}/retirar")]
    [ProducesResponseType(typeof(RetirarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Retirar(Guid id, [FromBody] RetirarRequest request, CancellationToken ct)
    {
        var command = new RetirarCommand(id, request.Monto, request.Ciudad);
        var result = await mediator.Send(command, ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : Ok(result.Value);
    }

    /// <summary>Retorna el detalle completo de una cuenta: saldo, tipo, cliente y ciudad.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CuentaDetalleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCuentaById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetCuentaByIdQuery(id), ct);
        return result.IsFailure
            ? NotFound(new ApiError(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta no encontrada", result.Error))
            : Ok(result.Value);
    }

    /// <summary>Consulta el saldo actual de la cuenta.</summary>
    [HttpGet("{id:guid}/saldo")]
    [ProducesResponseType(typeof(Application.Cuentas.Queries.SaldoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaldo(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSaldoQuery(id), ct);
        return result.IsFailure
            ? NotFound(new ApiError(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta no encontrada", result.Error))
            : Ok(result.Value);
    }

    /// <summary>Retorna los movimientos recientes de la cuenta con paginación.</summary>
    [HttpGet("{id:guid}/movimientos")]
    [ProducesResponseType(typeof(Application.Cuentas.Queries.MovimientosPagedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovimientos(
        Guid id,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetMovimientosQuery(id, pagina, tamano), ct);
        return result.IsFailure
            ? NotFound(new ApiError(ErrorCodes.ACCOUNT_NOT_FOUND, "Cuenta no encontrada", result.Error))
            : Ok(result.Value);
    }

    /// <summary>Genera el extracto mensual de la cuenta.</summary>
    [HttpGet("{id:guid}/extracto")]
    [ProducesResponseType(typeof(Application.Cuentas.Queries.ExtractoMensualDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExtracto(
        Guid id,
        [FromQuery] int mes,
        [FromQuery] int anio,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetExtractoMensualQuery(id, mes, anio), ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : Ok(result.Value);
    }
}
