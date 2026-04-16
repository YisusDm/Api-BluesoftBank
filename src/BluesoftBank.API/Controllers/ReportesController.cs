using BluesoftBank.API.Models;
using BluesoftBank.Application.Reportes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BluesoftBank.API.Controllers;

[ApiController]
[Route("api/reportes")]
[Produces("application/json")]
public sealed class ReportesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Clientes ordenados por número de transacciones en el mes (descendente).
    /// </summary>
    [HttpGet("top-clientes")]
    [ProducesResponseType(typeof(IReadOnlyList<TopClienteDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTopClientes(
        [FromQuery] int mes,
        [FromQuery] int anio,
        [FromQuery] int top = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetTopClientesQuery(mes, anio, top), ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : Ok(result.Value);
    }

    /// <summary>
    /// Clientes con retiros fuera de su ciudad de origen con valor total mayor a $1.000.000.
    /// </summary>
    [HttpGet("retiros-fuera-ciudad")]
    [ProducesResponseType(typeof(IReadOnlyList<RetiroFueraCiudadDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetRetirosFueraCiudad(
        [FromQuery] int? mes = null,
        [FromQuery] int? anio = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetRetirosFueraCiudadQuery(mes, anio), ct);
        return result.IsFailure
            ? BadRequest(new ApiError(ErrorCodes.INVALID_REQUEST, "Solicitud inválida", result.Error))
            : Ok(result.Value);
    }
}
