namespace BluesoftBank.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class SaldoInsuficienteException(decimal saldoDisponible, decimal montoSolicitado)
    : DomainException($"Saldo insuficiente. Disponible: {saldoDisponible:C}, solicitado: {montoSolicitado:C}.");

public sealed class MontoInvalidoException(decimal monto)
    : DomainException($"El monto '{monto}' es inválido. Debe ser mayor a cero.");

public sealed class CuentaNotFoundException(Guid cuentaId)
    : DomainException($"La cuenta con Id '{cuentaId}' no fue encontrada.");
