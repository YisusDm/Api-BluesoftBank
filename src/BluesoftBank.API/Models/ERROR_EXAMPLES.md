# Ejemplos de Mensajes de Error Mejorados

## 1. Saldo Insuficiente (422 - Unprocessable Entity)

### Antes:
```
"Saldo insuficiente. Disponible: $1,000.00, solicitado: $5,000.00."
```

### Ahora:
```json
{
  "code": "INSUFFICIENT_BALANCE",
  "title": "Fondos insuficientes",
  "message": "No tiene saldo suficiente para realizar esta operación. Saldo disponible: $1,000.00, monto solicitado: $5,000.00. Diferencia: $4,000.00",
  "suggestedAction": "Ingrese un monto menor o incremente el saldo de su cuenta",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## 2. Monto Mínimo de Retiro (422 - Unprocessable Entity)

### Antes:
```
"El monto mínimo de retiro es $1,000,000. Monto ingresado: $500,000."
```

### Ahora:
```json
{
  "code": "MINIMUM_WITHDRAWAL_NOT_MET",
  "title": "Monto de retiro inválido",
  "message": "El monto de retiro está por debajo del mínimo permitido. Monto mínimo: $1,000,000.00, monto ingresado: $500,000.00. Debe retirar al menos $1,000,000.00",
  "suggestedAction": "Aumente el monto del retiro al mínimo permitido",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## 3. Número de Cuenta Duplicado (409 - Conflict)

### Antes:
```
(Error de BD sin mensaje claro)
```

### Ahora:
```json
{
  "code": "DUPLICATE_ACCOUNT_NUMBER",
  "title": "Número de cuenta duplicado",
  "message": "El número de cuenta '12512111212' ya está registrado en el sistema. Ingrese un número de cuenta diferente",
  "suggestedAction": "Ingrese un número de cuenta diferente",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## 4. Correo Duplicado (409 - Conflict)

### Ahora:
```json
{
  "code": "DUPLICATE_EMAIL",
  "title": "Correo electrónico duplicado",
  "message": "El correo electrónico 'juan@email.com' ya está asociado a otra cuenta. Ingrese un correo diferente o use la cuenta existente",
  "suggestedAction": "Ingrese un correo electrónico diferente",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## 5. Cuenta No Encontrada (404 - Not Found)

### Antes:
```
"La cuenta con Id '550e8400-e29b-41d4-a716-446655440000' no fue encontrada."
```

### Ahora:
```json
{
  "code": "ACCOUNT_NOT_FOUND",
  "title": "Cuenta no encontrada",
  "message": "La cuenta solicitada no existe o ha sido eliminada. ID: 550e8400-e29b-41d4-a716-446655440000. Verifique que el número de cuenta sea correcto",
  "suggestedAction": "Verifique el ID de la cuenta e intente nuevamente",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## 6. Monto Inválido (400 - Bad Request)

### Antes:
```
"El monto '-1000' es inválido. Debe ser mayor a cero."
```

### Ahora:
```json
{
  "code": "INVALID_AMOUNT",
  "title": "Monto inválido",
  "message": "El monto ingresado (-$1,000.00) no es válido. Debe ingresar un monto mayor a cero (mínimo $1.000)",
  "suggestedAction": "Ingrese un monto válido mayor a cero",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## 7. Conflicto de Concurrencia (409 - Conflict)

### Antes:
```
(Error genérico SQL)
```

### Ahora:
```json
{
  "code": "CONCURRENCY_CONFLICT",
  "title": "Conflicto de concurrencia",
  "message": "La operación no pudo completarse porque el recurso fue modificado por otra operación simultánea",
  "suggestedAction": "Intente nuevamente en unos segundos",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## 8. Error Interno del Servidor (500 - Internal Server Error)

### Ahora:
```json
{
  "code": "INTERNAL_ERROR",
  "title": "Error interno del servidor",
  "message": "Ocurrió un error inesperado en el servidor",
  "suggestedAction": "Por favor intente más tarde o contacte al soporte",
  "timestamp": "2026-04-16T10:30:00Z"
}
```

---

## Códigos de Error Disponibles

| Código | HTTP Status | Significado |
|--------|------------|------------|
| `INVALID_AMOUNT` | 400 | Monto inválido |
| `INVALID_REQUEST` | 400 | Solicitud inválida |
| `ACCOUNT_NOT_FOUND` | 404 | Cuenta no encontrada |
| `CUSTOMER_NOT_FOUND` | 404 | Cliente no encontrado |
| `DUPLICATE_ACCOUNT_NUMBER` | 409 | Número de cuenta duplicado |
| `DUPLICATE_EMAIL` | 409 | Email duplicado |
| `DUPLICATE_KEY` | 409 | Datos duplicados |
| `CONCURRENCY_CONFLICT` | 409 | Conflicto de concurrencia |
| `INSUFFICIENT_BALANCE` | 422 | Saldo insuficiente |
| `MINIMUM_WITHDRAWAL_NOT_MET` | 422 | Monto mínimo no alcanzado |
| `BUSINESS_RULE_VIOLATION` | 422 | Violación de regla de negocio |
| `INTERNAL_ERROR` | 500 | Error interno del servidor |

---

## Ventajas del Nuevo Sistema

✅ **Mensajes claros y específicos** - El cliente entiende exactamente qué salió mal  
✅ **Códigos de error estandarizados** - Fácil de procesar en la aplicación cliente  
✅ **Acciones sugeridas** - El cliente sabe qué hacer después  
✅ **Timestamp** - Para auditoría y debugging  
✅ **Estructura consistente** - Todos los errores siguen el mismo formato  
