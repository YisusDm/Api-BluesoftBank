namespace BluesoftBank.Domain.Exceptions;

public abstract class DomainException(string message) : Exception(message);

public sealed class SaldoInsuficienteException(decimal saldoDisponible, decimal montoSolicitado)
    : DomainException(
        $"No tiene saldo suficiente para realizar esta operación. " +
        $"Saldo disponible: {saldoDisponible:C2}, " +
        $"monto solicitado: {montoSolicitado:C2}. " +
        $"Diferencia: {(montoSolicitado - saldoDisponible):C2}");

public sealed class MontoInvalidoException(decimal monto)
    : DomainException(
        $"El monto ingresado ({monto:C2}) no es válido. " +
        $"Debe ingresar un monto mayor a cero (mínimo $1.000).");

public sealed class CuentaNotFoundException(Guid cuentaId)
    : DomainException(
        $"La cuenta solicitada no existe o ha sido eliminada. " +
        $"ID: {cuentaId}. " +
        $"Verifique que el número de cuenta sea correcto.");

public sealed class MontoMinimoRetiroException(decimal montoIntentado, decimal montoMinimo)
    : DomainException(
        $"El monto de retiro está por debajo del mínimo permitido. " +
        $"Monto mínimo: {montoMinimo:C2}, " +
        $"monto ingresado: {montoIntentado:C2}. " +
        $"Debe retirar al menos {montoMinimo:C2}.");

public sealed class NumeroCuentaDuplicadoException(string numeroCuenta)
    : DomainException(
        $"El número de cuenta '{numeroCuenta}' ya está registrado en el sistema. " +
        $"Ingrese un número de cuenta diferente.");

public sealed class CorreoDuplicadoException(string correo)
    : DomainException(
        $"El correo electrónico '{correo}' ya está asociado a otra cuenta. " +
        $"Ingrese un correo diferente o use la cuenta existente.");

public sealed class ClienteYaExisteException(string identificacion)
    : DomainException(
        $"Ya existe un cliente registrado con la identificación '{identificacion}'. " +
        $"Puede crear una nueva cuenta para este cliente.");
