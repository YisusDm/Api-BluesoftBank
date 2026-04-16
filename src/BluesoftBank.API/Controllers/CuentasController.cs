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
    /// <summary>Realiza una consignación en la cuenta especificada.</summary>
    [HttpPost("{id:guid}/consignar")]
    [ProducesResponseType(typeof(ConsignarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Consignar(Guid id, [FromBody] ConsignarRequest request, CancellationToken ct)
    {
        var command = new ConsignarCommand(id, request.Monto, request.Ciudad);
        var result = await mediator.Send(command, ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }

    /// <summary>Realiza un retiro de la cuenta especificada.</summary>
    [HttpPost("{id:guid}/retirar")]
    [ProducesResponseType(typeof(RetirarResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Retirar(Guid id, [FromBody] RetirarRequest request, CancellationToken ct)
    {
        var command = new RetirarCommand(id, request.Monto, request.Ciudad);
        var result = await mediator.Send(command, ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }

    /// <summary>Consulta el saldo actual de la cuenta.</summary>
    [HttpGet("{id:guid}/saldo")]
    [ProducesResponseType(typeof(Application.Cuentas.Queries.SaldoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSaldo(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetSaldoQuery(id), ct);
        return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
    }

    /// <summary>Retorna los movimientos recientes de la cuenta con paginación.</summary>
    [HttpGet("{id:guid}/movimientos")]
    [ProducesResponseType(typeof(Application.Cuentas.Queries.MovimientosPagedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovimientos(
        Guid id,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamano = 20,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetMovimientosQuery(id, pagina, tamano), ct);
        return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
    }

    /// <summary>Genera el extracto mensual de la cuenta.</summary>
    [HttpGet("{id:guid}/extracto")]
    [ProducesResponseType(typeof(Application.Cuentas.Queries.ExtractoMensualDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExtracto(
        Guid id,
        [FromQuery] int mes,
        [FromQuery] int anio,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetExtractoMensualQuery(id, mes, anio), ct);
        return result.IsFailure ? BadRequest(result.Error) : Ok(result.Value);
    }
}

public sealed record ConsignarRequest(decimal Monto, string Ciudad);
public sealed record RetirarRequest(decimal Monto, string Ciudad);
