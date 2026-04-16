namespace BluesoftBank.API.Models;

/// <summary>
/// Modelo de respuesta de error API estándar
/// </summary>
public sealed record ApiError(
    string Code,
    string Title,
    string Message,
    string? SuggestedAction = null,
    Dictionary<string, string[]>? ValidationErrors = null,
    DateTime Timestamp = default)
{
    public ApiError() : this(
        Code: "UNKNOWN_ERROR",
        Title: "Error desconocido",
        Message: "Ocurrió un error inesperado",
        SuggestedAction: "Intente nuevamente",
        Timestamp: DateTime.UtcNow)
    {
    }
};

/// <summary>
/// Códigos de error estandarizados
/// </summary>
public static class ErrorCodes
{
    // Errores de validación (400)
    public const string INVALID_AMOUNT = "INVALID_AMOUNT";
    public const string INVALID_REQUEST = "INVALID_REQUEST";

    // Errores de no encontrado (404)
    public const string ACCOUNT_NOT_FOUND = "ACCOUNT_NOT_FOUND";
    public const string CUSTOMER_NOT_FOUND = "CUSTOMER_NOT_FOUND";

    // Errores de conflicto (409)
    public const string DUPLICATE_ACCOUNT_NUMBER = "DUPLICATE_ACCOUNT_NUMBER";
    public const string DUPLICATE_EMAIL = "DUPLICATE_EMAIL";
    public const string DUPLICATE_KEY = "DUPLICATE_KEY";
    public const string CONCURRENCY_CONFLICT = "CONCURRENCY_CONFLICT";

    // Errores de regla de negocio (422)
    public const string INSUFFICIENT_BALANCE = "INSUFFICIENT_BALANCE";
    public const string MINIMUM_WITHDRAWAL_NOT_MET = "MINIMUM_WITHDRAWAL_NOT_MET";
    public const string BUSINESS_RULE_VIOLATION = "BUSINESS_RULE_VIOLATION";

    // Errores de servidor (500)
    public const string INTERNAL_ERROR = "INTERNAL_ERROR";
}
