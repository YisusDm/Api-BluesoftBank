using BluesoftBank.API.Models;
using BluesoftBank.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace BluesoftBank.API.Middlewares;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
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
        var (status, errorCode, title, message, suggestedAction) = exception switch
        {
            CuentaNotFoundException ex =>
                (StatusCodes.Status404NotFound,
                 ErrorCodes.ACCOUNT_NOT_FOUND,
                 "Cuenta no encontrada",
                 ex.Message,
                 "Verifique el ID de la cuenta e intente nuevamente"),

            MontoMinimoRetiroException ex =>
                (StatusCodes.Status422UnprocessableEntity,
                 ErrorCodes.MINIMUM_WITHDRAWAL_NOT_MET,
                 "Monto de retiro inválido",
                 ex.Message,
                 "Aumente el monto del retiro al mínimo permitido"),

            SaldoInsuficienteException ex =>
                (StatusCodes.Status422UnprocessableEntity,
                 ErrorCodes.INSUFFICIENT_BALANCE,
                 "Fondos insuficientes",
                 ex.Message,
                 "Ingrese un monto menor o incremente el saldo de su cuenta"),

            MontoInvalidoException ex =>
                (StatusCodes.Status400BadRequest,
                 ErrorCodes.INVALID_AMOUNT,
                 "Monto inválido",
                 ex.Message,
                 "Ingrese un monto válido mayor a cero"),

            NumeroCuentaDuplicadoException ex =>
                (StatusCodes.Status409Conflict,
                 ErrorCodes.DUPLICATE_ACCOUNT_NUMBER,
                 "Número de cuenta duplicado",
                 ex.Message,
                 "Ingrese un número de cuenta diferente"),

            CorreoDuplicadoException ex =>
                (StatusCodes.Status409Conflict,
                 ErrorCodes.DUPLICATE_EMAIL,
                 "Correo electrónico duplicado",
                 ex.Message,
                 "Ingrese un correo electrónico diferente"),

            ClienteYaExisteException ex =>
                (StatusCodes.Status409Conflict,
                 ErrorCodes.DUPLICATE_KEY,
                 "Cliente ya registrado",
                 ex.Message,
                 "Intente registrarse con datos diferentes o use el cliente existente"),

            SqlException sqlEx when (sqlEx.Number == 1205) =>
                (StatusCodes.Status409Conflict,
                 ErrorCodes.CONCURRENCY_CONFLICT,
                 "Conflicto de concurrencia",
                 "La operación no pudo completarse porque el recurso fue modificado por otra operación simultánea.",
                 "Intente nuevamente en unos segundos"),

            SqlException sqlEx when (sqlEx.Number == 2627) || (sqlEx.Number == 2601) =>
                (StatusCodes.Status409Conflict,
                 ErrorCodes.DUPLICATE_KEY,
                 "Datos duplicados",
                 ExtractDuplicateKeyMessage(sqlEx),
                 "Verifique los datos e intente nuevamente"),

            DomainException ex =>
                (StatusCodes.Status422UnprocessableEntity,
                 ErrorCodes.BUSINESS_RULE_VIOLATION,
                 "Error en la operación",
                 ex.Message,
                 "Verifique los datos e intente nuevamente"),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 ErrorCodes.INTERNAL_ERROR,
                 "Error interno del servidor",
                 "Ocurrió un error inesperado en el servidor.",
                 "Por favor intente más tarde o contacte al soporte")
        };

        var error = new ApiError(
            Code: errorCode,
            Title: title,
            Message: message,
            SuggestedAction: suggestedAction,
            Timestamp: DateTime.UtcNow);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = status;

        return context.Response.WriteAsync(JsonSerializer.Serialize(error, _jsonOptions));
    }

    private static string ExtractDuplicateKeyMessage(SqlException ex)
    {
        return ex.Message.Contains("NumeroCuenta")
            ? "El número de cuenta ingresado ya existe en el sistema."
            : ex.Message.Contains("Correo")
            ? "El correo electrónico ingresado ya está registrado."
            : "Los datos ingresados ya existen en el sistema.";
    }
}
