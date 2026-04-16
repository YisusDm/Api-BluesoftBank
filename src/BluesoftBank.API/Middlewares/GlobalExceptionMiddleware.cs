using BluesoftBank.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BluesoftBank.API.Middlewares;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (status, title) = exception switch
        {
            CuentaNotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            SaldoInsuficienteException => (StatusCodes.Status422UnprocessableEntity, "Regla de negocio violada"),
            MontoInvalidoException => (StatusCodes.Status400BadRequest, "Solicitud inválida"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor")
        };

        var detail = exception is DomainException
            ? exception.Message
            : "Ocurrió un error inesperado. Por favor intente nuevamente.";

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
