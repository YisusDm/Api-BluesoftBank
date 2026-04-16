# 🏦 BluesoftBank API

![.NET 10](https://img.shields.io/badge/.NET-10-512BD4)
![C# 13](https://img.shields.io/badge/C%23-13-green)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019%2B-CC2927)
![License](https://img.shields.io/badge/License-MIT-blue)

Una API REST backend moderna, escalable y segura para gestión de cuentas bancarias con arquitectura limpia, CQRS y garantías de concurrencia transaccional. Construida con **.NET 10**, **SQL Server** y patrones empresariales de alto rendimiento.

---

## 📋 Tabla de Contenidos

1. [Descripción General](#descripción-general)
2. [Stack Tecnológico](#stack-tecnológico)
3. [Arquitectura](#arquitectura)
4. [Instalación y Configuración](#instalación-y-configuración)
5. [Migraciones de Base de Datos](#migraciones-de-base-de-datos)
6. [Ejecución](#ejecución)
7. [API Endpoints](#api-endpoints)
8. [Seguridad Transaccional](#seguridad-transaccional)
9. [Testing](#testing)
10. [Estructura del Proyecto](#estructura-del-proyecto)
11. [Documentación](#documentación)

---

## Descripción General

**BluesoftBank API** es una solución backend que implementa patrones empresariales modernos para sistemas financieros:

✅ **Clean Architecture** — Separación clara de responsabilidades en capas aisladas  
✅ **CQRS Pattern** — Segregación de comandos (escritura) y queries (lectura)  
✅ **Concurrencia Segura** — Bloqueos pessimistas + transacciones Serializable  
✅ **Validación Robusta** — FluentValidation + Guard clauses en el dominio  
✅ **Result Pattern** — Manejo de errores funcional y tipado  
✅ **Escalabilidad** — Índices optimizados, paginación, queries eficientes  
✅ **Testeable** — Inyección de dependencias, interfaces, cobertura de tests  
✅ **Documentado** — Swagger/OpenAPI + comentarios XML

### Casos de Uso

- ✓ Crear cuentas bancarias (Ahorro y Corriente)
- ✓ Consignar y retirar dinero de forma segura
- ✓ Consultar saldo y movimientos
- ✓ Generar reportes (extracto mensual, retiros fuera de ciudad)
- ✓ Manejar clientes (PersonaNatural y Empresa)
- ✓ Garantizar transacciones ACID bajo concurrencia

---

## Stack Tecnológico

### Runtime y Lenguaje

| Componente | Versión | Descripción |
|---|---|---|
| **.NET** | 10 | Runtime LTS moderno |
| **C#** | 13 | Records, nullable reference types, top-level statements |
| **Target Framework** | net10.0 | Framework de destino actual |

### Base de Datos

| Componente | Versión | Rol |
|---|---|---|
| **SQL Server** | 2019+ | RDBMS relacional con soporte ACID |
| **Entity Framework Core** | 10 | ORM para abstracción de acceso a datos |
| **Migrations** | Automáticas | Versionado del esquema de BD |

### Patrones y Librerías

| Librería | Versión | Propósito |
|---|---|---|
| **MediatR** | 12 | Implementación de CQRS + Pipeline Behaviors |
| **FluentValidation** | 11 | Validación fluida y reutilizable |
| **Swashbuckle** | 6+ | Documentación API con Swagger/OpenAPI 3.0 |
| **xUnit** | - | Framework de pruebas |
| **Moq** | - | Mocking de dependencias |
| **FluentAssertions** | - | Assertions legibles en tests |

---

## Arquitectura

### Diagrama en Capas (Clean Architecture)

```
┌────────────────────────────────────────────────────┐
│                    API LAYER                        │
│  Controllers, Routing, Middleware, HTTP            │
│  Dependencia: Application, Infrastructure          │
└────────────────────────────────────────────────────┘
                          ▲
                          │
┌────────────────────────────────────────────────────┐
│              APPLICATION LAYER                      │
│  CQRS Handlers, DTOs, Validators, Behaviors        │
│  Dependencia: Domain, Infrastructure               │
└────────────────────────────────────────────────────┘
                          ▲
                          │
┌────────────────────────────────────────────────────┐
│            INFRASTRUCTURE LAYER                     │
│  EF Core, Repositories, DbContext, UnitOfWork     │
│  Dependencia: Domain                               │
└────────────────────────────────────────────────────┘
                          ▲
                          │
┌────────────────────────────────────────────────────┐
│               DOMAIN LAYER                          │
│  Entidades, Value Objects, Domain Events          │
│  Dependencia: NADA (puro, aislado)                │
└────────────────────────────────────────────────────┘
```

### Patrón CQRS

```
HTTP Request
    │
    ├─────────────────┬─────────────────┐
    │                 │                 │
    ▼                 ▼                 ▼
┌──────────┐      ┌──────────┐      ┌──────────┐
│ COMMAND  │      │  QUERY   │      │ RESPONSE │
│(Escribir)│      │ (Leer)   │      │  (DTO)   │
└──────────┘      └──────────┘      └──────────┘
    │                 │                 │
    │                 │                 │
    └─────────────────┴─────────────────┘
            │
            ▼
    ┌─────────────────┐
    │   MediatR       │
    │   Pipeline      │
    └─────────────────┘
            │
    ┌───────┴───────┐
    │               │
    ▼               ▼
Logging        Validation
    │               │
    └───────┬───────┘
            │
            ▼
    ┌─────────────────┐
    │  Handler        │
    │ (Lógica Real)   │
    └─────────────────┘
            │
            ▼
    ┌─────────────────┐
    │  Repositories   │
    │  Domain Logic   │
    └─────────────────┘
            │
            ▼
        DATABASE
```

### Decisiones Arquitectónicas Clave

| Decisión | Por Qué |
|---|---|
| **CQRS con MediatR** | Separación clara, testeable, escalable |
| **Clean Architecture** | Aislamiento de concerns, mantenibilidad |
| **TPH (Table Per Hierarchy)** | Rendimiento: sin JOINs, solo 2 tablas |
| **Bloqueo Pessimista** | Ambiente bancario: coherencia > throughput |
| **Serializable Isolation** | Garantiza ACID bajo concurrencia máxima |
| **Value Objects** | Autovalidación, type-safety |
| **Result Pattern** | Errores tipados sin excepciones |
| **Domain Events** | Auditoría desacoplada, extensibilidad |

---

## Instalación y Configuración

### Prerequisitos

Asegúrate de tener instalado:

| Herramienta | Versión | Descarga |
|---|---|---|
| .NET SDK | 10.0+ | https://dot.net/download |
| SQL Server | 2019+ | Descarga o Docker |
| EF Core CLI | 10.0+ | `dotnet tool install -g dotnet-ef` |
| Git | Última | https://git-scm.com |

### Paso 1: Clonar el Repositorio

```bash
git clone https://github.com/your-org/Api-BluesoftBank.git
cd Api-BluesoftBank
```

### Paso 2: Configurar SQL Server (Opcional con Docker)

Si usas Docker, inicia SQL Server:

```bash
docker run -e "ACCEPT_EULA=Y" \
  -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 \
  --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### Paso 3: Configurar Cadena de Conexión

Edita `src/BluesoftBank.API/appsettings.Development.json`:

#### Opción A: SQL Server en Docker

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=BluesoftBank;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

#### Opción B: SQL Server Local (Autenticación Windows)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BluesoftBank;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### Opción C: Azure SQL

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=myserver.database.windows.net;Database=BluesoftBank;User Id=admin@myserver;Password=YourStrong@Passw0rd;"
  }
}
```

### Paso 4: Restaurar Dependencias

```bash
dotnet restore
```

---

## Migraciones de Base de Datos

### Crear la Base de Datos Inicial

```bash
dotnet ef database update \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API
```

Esto ejecutará todas las migraciones pendientes y creará el esquema.

### Crear una Nueva Migración (Después de cambios en el modelo)

```bash
dotnet ef migrations add [NombreDeLaMigracion] \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API
```

**Ejemplo:**

```bash
dotnet ef migrations add AddColumnToTransaccion \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API
```

### Ver Migraciones Pendientes

```bash
dotnet ef migrations list \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API
```

### Revertir a una Migración Anterior

```bash
dotnet ef database update [NombreDeLaMigracion] \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API
```

**Ejemplo:**

```bash
dotnet ef database update InitialCreate \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API
```

### Esquema de Base de Datos

La API crea automáticamente estas tablas:

```sql
-- Tabla de Clientes (PersonaNatural y Empresa)
CREATE TABLE Clientes (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Discriminator NVARCHAR(50),        -- "PersonaNatural" o "Empresa"
    Nombre NVARCHAR(200) NOT NULL,
    Correo NVARCHAR(200) UNIQUE NOT NULL,
    Cedula NVARCHAR(20) UNIQUE NULL,   -- Para PersonaNatural
    NIT NVARCHAR(20) UNIQUE NULL,      -- Para Empresa
    CiudadNombre NVARCHAR(100) NOT NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Tabla de Cuentas (CuentaAhorro y CuentaCorriente)
CREATE TABLE Cuentas (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Discriminator NVARCHAR(50),        -- "CuentaAhorro" o "CuentaCorriente"
    NumeroCuenta NVARCHAR(20) UNIQUE NOT NULL,
    Saldo DECIMAL(18,2) NOT NULL,
    CupoSobregiro DECIMAL(18,2) NULL,  -- Solo CuentaCorriente
    ClienteId UNIQUEIDENTIFIER NOT NULL,
    CiudadNombre NVARCHAR(100) NOT NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Tabla de Transacciones
CREATE TABLE Transacciones (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CuentaId UNIQUEIDENTIFIER NOT NULL,
    Tipo INT NOT NULL,                 -- 0=Consignacion, 1=Retiro
    Monto DECIMAL(18,2) NOT NULL,
    SaldoResultante DECIMAL(18,2) NOT NULL,
    CiudadNombre NVARCHAR(100) NOT NULL,
    EsFueraDeCiudadOrigen BIT NOT NULL,
    Fecha DATETIME2(7) NOT NULL
);

-- Índices para rendimiento
CREATE INDEX IX_Transaccion_Cuenta_Fecha ON Transacciones(CuentaId, Fecha);
CREATE INDEX IX_Transaccion_FueraCiudad_Fecha ON Transacciones(EsFueraDeCiudadOrigen, Fecha);
```

---

## Ejecución

### Modo Desarrollo

```bash
dotnet run --project src/BluesoftBank.API
```

La API estará disponible en:

- **HTTP:** http://localhost:5082
- **HTTPS:** https://localhost:7247
- **Swagger UI:** https://localhost:7247/swagger

### Modo Release

```bash
dotnet publish src/BluesoftBank.API -c Release -o ./publish
dotnet ./publish/BluesoftBank.API.dll
```

### Con Docker

```bash
# 1. Construir imagen
docker build -t bluesoft-bank-api .

# 2. Ejecutar contenedor
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=BluesoftBank;..." \
  bluesoft-bank-api
```

---

## API Endpoints

### Base URL

```
https://localhost:7247
Content-Type: application/json
```

### 📌 Cuentas

#### 1. Listar Cuentas (Paginado)

```http
GET /api/cuentas?pagina=1&tamano=10
```

**Response 200 OK:**

```json
{
  "cuentas": [
    {
      "cuentaId": "550e8400-e29b-41d4-a716-446655440000",
      "numeroCuenta": "ACC-0001",
      "tipoCuenta": "CuentaAhorro",
      "saldo": 5000000.00,
      "fechaCreacion": "2026-04-16T10:30:00Z",
      "cliente": {
        "clienteId": "660e8400-e29b-41d4-a716-446655440001",
        "nombre": "Juan García",
        "correo": "juan@email.com",
        "ciudad": "Bogotá"
      }
    }
  ],
  "totalRegistros": 1,
  "pagina": 1,
  "tamano": 10
}
```

#### 2. Obtener Detalle de Cuenta

```http
GET /api/cuentas/{id}
```

**Response 200 OK:**

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "numeroCuenta": "AHO-2026-001",
  "tipoCuenta": "CuentaAhorro",
  "saldo": 5000000.00,
  "ciudad": "Bogotá",
  "fechaCreacion": "2026-04-16T10:30:00Z",
  "nombreCliente": "Juan García",
  "clienteId": "660e8400-e29b-41d4-a716-446655440001",
  "correoCliente": "juan@email.com",
  "ciudadCliente": "Bogotá"
}
```

#### 3. Crear Cuenta Ahorro

```http
POST /api/cuentas/ahorro
Content-Type: application/json

{
  "nombreCliente": "Juan García López",
  "cedulaCliente": "1.234.567",
  "correoCliente": "juan@email.com",
  "ciudadCuenta": "Bogotá"
}
```

**Response 201 Created:**

```json
{
  "cuentaId": "550e8400-e29b-41d4-a716-446655440000",
  "numeroCuenta": "AHO-2026-001",
  "saldo": 0.00
}
```

#### 4. Crear Cuenta Corriente

```http
POST /api/cuentas/corriente
Content-Type: application/json

{
  "nombreCliente": "Tech Solutions SAS",
  "nitCliente": "12.345.678-1",
  "correoCliente": "contacto@techsolutions.com",
  "ciudadCuenta": "Medellín",
  "cupoSobregiro": 10000000
}
```

**Response 201 Created:**

```json
{
  "cuentaId": "550e8400-e29b-41d4-a716-446655440000",
  "numeroCuenta": "COR-2026-001",
  "saldo": 0.00,
  "cupoSobregiro": 10000000.00
}
```

#### 5. Consignar Dinero

```http
POST /api/cuentas/{id}/consignar
Content-Type: application/json

{
  "monto": 5000000,
  "ciudad": "Bogotá"
}
```

**Response 200 OK:**

```json
{
  "cuentaId": "550e8400-e29b-41d4-a716-446655440000",
  "nuevoSaldo": 5000000.00,
  "transaccionId": "770e8400-e29b-41d4-a716-446655440002",
  "fechaOperacion": "2026-04-16T10:35:00Z"
}
```

#### 6. Retirar Dinero

```http
POST /api/cuentas/{id}/retirar
Content-Type: application/json

{
  "monto": 1000000,
  "ciudad": "Medellín"
}
```

**Response 200 OK:**

```json
{
  "cuentaId": "550e8400-e29b-41d4-a716-446655440000",
  "nuevoSaldo": 4000000.00,
  "transaccionId": "770e8400-e29b-41d4-a716-446655440003",
  "esFueraDeCiudadOrigen": true,
  "fechaOperacion": "2026-04-16T10:40:00Z"
}
```

#### 7. Consultar Saldo

```http
GET /api/cuentas/{id}/saldo
```

**Response 200 OK:**

```json
{
  "saldo": 4000000.00,
  "moneda": "COP",
  "fechaConsulta": "2026-04-16T10:45:00Z"
}
```

#### 8. Obtener Movimientos (Paginado)

```http
GET /api/cuentas/{id}/movimientos?pagina=1&tamano=20
```

**Response 200 OK:**

```json
{
  "cuentaId": "550e8400-e29b-41d4-a716-446655440000",
  "movimientos": [
    {
      "id": "770e8400-e29b-41d4-a716-446655440003",
      "tipo": "Retiro",
      "monto": 1000000.00,
      "saldoResultante": 4000000.00,
      "ciudad": "Medellín",
      "esFueraDeCiudadOrigen": true,
      "fecha": "2026-04-16T10:40:00Z"
    },
    {
      "id": "770e8400-e29b-41d4-a716-446655440002",
      "tipo": "Consignacion",
      "monto": 5000000.00,
      "saldoResultante": 5000000.00,
      "ciudad": "Bogotá",
      "esFueraDeCiudadOrigen": false,
      "fecha": "2026-04-16T10:35:00Z"
    }
  ],
  "totalMovimientos": 2,
  "pagina": 1,
  "tamano": 20
}
```

#### 9. Extracto Mensual

```http
GET /api/cuentas/{id}/extracto?mes=4&anio=2026
```

**Response 200 OK:**

```json
{
  "cuentaId": "550e8400-e29b-41d4-a716-446655440000",
  "numeroCuenta": "AHO-2026-001",
  "periodo": "Abril 2026",
  "saldoInicial": 0.00,
  "saldoFinal": 4000000.00,
  "totalConsignaciones": 5000000.00,
  "totalRetiros": 1000000.00,
  "movimientos": [
    {
      "id": "770e8400-e29b-41d4-a716-446655440002",
      "tipo": "Consignacion",
      "monto": 5000000.00,
      "saldoResultante": 5000000.00,
      "ciudad": "Bogotá",
      "esFueraDeCiudadOrigen": false,
      "fecha": "2026-04-16T10:35:00Z"
    }
  ]
}
```

### 📊 Reportes

#### 10. Top Clientes

```http
GET /api/reportes/top-clientes?mes=4&anio=2026&top=5
```

**Response 200 OK:**

```json
{
  "periodo": "Abril 2026",
  "clientes": [
    {
      "clienteId": "660e8400-e29b-41d4-a716-446655440001",
      "nombre": "Juan García",
      "tipo": "PersonaNatural",
      "totalTransacciones": 15,
      "montoTotalMovido": 25000000.00
    }
  ]
}
```

#### 11. Retiros Fuera de Ciudad

```http
GET /api/reportes/retiros-fuera-ciudad?mes=4&anio=2026&minimoMonto=1000000
```

**Response 200 OK:**

```json
{
  "periodo": "Abril 2026",
  "retiros": [
    {
      "transaccionId": "770e8400-e29b-41d4-a716-446655440003",
      "numeroCuenta": "AHO-2026-001",
      "nombreCliente": "Juan García",
      "monto": 1000000.00,
      "ciudadOrigen": "Bogotá",
      "ciudadRetiro": "Medellín",
      "fecha": "2026-04-16T10:40:00Z"
    }
  ],
  "totalRetiros": 1,
  "montoTotal": 1000000.00
}
```

---

## Seguridad Transaccional

### Problema: Concurrencia en Retiros

```plaintext
Thread 1: Consulta saldo = $1,000
Thread 2: Consulta saldo = $1,000
          
Thread 1: Retira $500 → Saldo = $500
Thread 2: Retira $700 → Saldo = $300 ❌ INCORRECTO

Resultado esperado: $1,000 - $500 - $700 = -$200 o error de saldo insuficiente
Resultado obtenido: $300 (dinero desaparecido)
```

### Solución Implementada

#### 1️⃣ Bloqueo Pessimista (UPDLOCK + ROWLOCK)

```sql
SELECT * FROM Cuentas WITH (UPDLOCK, ROWLOCK) WHERE Id = @id
-- La fila se bloquea para lectura y escritura exclusivamente
```

#### 2️⃣ Transacción Serializable

```csharp
await unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
// Aislamiento máximo: otros threads esperan
```

#### 3️⃣ Validación en Dominio

```csharp
public void Retirar(Dinero monto, Ciudad ciudad)
{
    if (monto.Valor < 1_000_000) 
        throw new MontoMinimoRetiroException();
    
    ValidarRetiro(monto);  // Template Method: reglas específicas por tipo
    
    Saldo -= monto.Valor;  // Decremento atómico
}
```

#### 4️⃣ Commit Atómico

```csharp
await unitOfWork.CommitAsync();  // Libera el bloqueo
```

### Flujo Seguro Completo

```
POST /api/cuentas/{id}/retirar
    ↓
RetirarCommandHandler.Handle()
    ↓
UnitOfWork.BeginTransaction(Serializable)
    ├─ SQL: SELECT * FROM Cuentas WITH (UPDLOCK, ROWLOCK) WHERE Id = {id}
    │       → Fila bloqueada, otros threads esperan
    │
    ├─ Cuenta.Retirar(monto, ciudad)
    │   ├─ ValidarRetiro(): if (Saldo < monto) throw Exception
    │   └─ Saldo -= monto
    │
    ├─ Transaccion creada y agregada
    │
    ├─ SaveChangesAsync()
    │   └─ UPDATE Cuentas SET Saldo = @nuevoSaldo WHERE Id = @id
    │
    └─ CommitAsync()
       └─ COMMIT TRANSACTION (libera bloqueo)
    
✅ Operación atómica garantizada
```

---

## Testing

### Ejecutar Todos los Tests

```bash
dotnet test
```

### Tests con Output Detallado

```bash
dotnet test --logger "console;verbosity=detailed"
```

### Tests Solo del Dominio (Rápidos)

```bash
dotnet test tests/BluesoftBank.Domain.Tests
```

### Tests Solo de Aplicación

```bash
dotnet test tests/BluesoftBank.Application.Tests
```

### Cobertura de Tests

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=json
```

### Ejemplo: Test de Consignación Concurrente

```csharp
[Fact]
public async Task Consignar_ConcurrentRequests_SumaCorrecta()
{
    // ARRANGE
    var cuenta = new CuentaAhorro("001", new Ciudad("BOGOTA"), Guid.NewGuid());
    var monto = new Dinero(1_000_000);
    
    // ACT: Simular 100 consignaciones concurrentes
    var tasks = Enumerable.Range(1, 100)
        .Select(_ => Task.Run(() => cuenta.Consignar(monto)));
    
    await Task.WhenAll(tasks);
    
    // ASSERT
    Assert.Equal(100_000_000, cuenta.Saldo);  // 100 * 1,000,000
    Assert.Equal(100, cuenta.Transacciones.Count);
}
```

---

## Estructura del Proyecto

```
Api-BluesoftBank/
├── src/
│   ├── BluesoftBank.Domain/              ← Dominio (puro, sin dependencias)
│   │   ├── Entities/                     (Cuenta, Cliente, Transaccion)
│   │   ├── ValueObjects/                 (Dinero, Ciudad)
│   │   ├── Events/                       (Domain Events para auditoría)
│   │   ├── Exceptions/                   (DomainException, custom)
│   │   └── Enums/                        (TipoTransaccion)
│   │
│   ├── BluesoftBank.Application/         ← Aplicación (CQRS, DTOs)
│   │   ├── Cuentas/
│   │   │   ├── Commands/                 (CrearCuenta, Consignar, Retirar)
│   │   │   ├── Queries/                  (GetCuentas, GetMovimientos, etc)
│   │   │   ├── Dtos/                     (CuentaListItemDto, MovimientoDto)
│   │   │   └── Validators/               (Validadores FluentValidation)
│   │   ├── Reportes/
│   │   │   ├── Queries/                  (GetTopClientes, GetRetirosFueraCiudad)
│   │   │   └── Dtos/
│   │   ├── Common/
│   │   │   ├── Behaviors/                (LoggingBehavior, ValidationBehavior)
│   │   │   ├── Interfaces/               (Repository interfaces)
│   │   │   └── Results/                  (Result<T> pattern)
│   │   └── DependencyInjection.cs
│   │
│   ├── BluesoftBank.Infrastructure/      ← Infraestructura (EF Core, Repos)
│   │   ├── Persistence/
│   │   │   ├── Configurations/           (EF Core EntityTypeConfigurations)
│   │   │   ├── Repositories/             (CuentaRepository, ClienteRepository)
│   │   │   └── Migrations/               (Migraciones de BD)
│   │   ├── BankDbContext.cs              (DbContext)
│   │   ├── UnitOfWork.cs                 (Patrón UnitOfWork)
│   │   └── DependencyInjection.cs
│   │
│   └── BluesoftBank.API/                 ← API REST
│       ├── Controllers/                  (CuentasController, ReportesController)
│       ├── Middlewares/                  (GlobalExceptionMiddleware)
│       ├── Models/                       (Request/Response models)
│       ├── Program.cs                    (Configuración DI, middlewares)
│       ├── appsettings.json
│       └── appsettings.Development.json
│
├── tests/
│   ├── BluesoftBank.Domain.Tests/        ← Tests de dominio
│   │   └── Entities/
│   │
│   └── BluesoftBank.Application.Tests/   ← Tests de aplicación
│       └── Handlers/
│
├── docs/                                 ← Documentación
│   ├── ARQUITECTURA_TECNOLOGIAS.md
│   ├── API_DOCUMENTATION.md
│   ├── setup.md
│   └── ...
│
├── README.md                             ← Este archivo
└── Api-BluesoftBank.slnx                 ← Solution file
```

---

## Documentación

### Documentación de API (Swagger)

Una vez que la API esté en ejecución, accede a:

```
https://localhost:7247/swagger
```

Aquí puedes:
- ✅ Ver todos los endpoints
- ✅ Leer parámetros y respuestas
- ✅ Probar endpoints interactivamente

### Documentación Técnica

Consulta la carpeta `docs/` para información detallada:

- **`ARQUITECTURA_TECNOLOGIAS.md`** — Arquitectura completa, patrones, tecnologías
- **`API_DOCUMENTATION.md`** — Especificación completa de endpoints
- **`setup.md`** — Instrucciones de instalación y configuración
- **`MODELO_DOMINIO.md`** — Modelo de dominio y entidades
- **`concurrencia.md`** — Detalles de seguridad transaccional

---

## Troubleshooting

| Problema | Causa | Solución |
|---|---|---|
| `Cannot open database "BluesoftBank"` | BD no creada | Ejecutar `dotnet ef database update` |
| `A connection was successfully established... SSL` | Certificado no confiable | Agregar `TrustServerCertificate=True` a la cadena |
| `dotnet ef: command not found` | EF CLI no instalada | `dotnet tool install -g dotnet-ef` |
| `DbUpdateConcurrencyException` | Conflicto de concurrencia | Reintentar operación (ver Seguridad Transaccional) |
| Puerto 5082/7247 en uso | Otro proceso ocupa el puerto | Cambiar puerto en `launchSettings.json` |

---

## Comandos Rápidos

```bash
# Restaurar dependencias
dotnet restore

# Construir proyecto
dotnet build

# Crear migración
dotnet ef migrations add [NombreMigracion] \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API

# Aplicar migraciones
dotnet ef database update \
  --project src/BluesoftBank.Infrastructure \
  --startup-project src/BluesoftBank.API

# Ejecutar API
dotnet run --project src/BluesoftBank.API

# Ejecutar tests
dotnet test

# Publicar release
dotnet publish src/BluesoftBank.API -c Release -o ./publish
```

---

## Contribución

1. Crea una rama: `git checkout -b feature/mi-feature`
2. Realiza cambios y commits
3. Push a la rama: `git push origin feature/mi-feature`
4. Abre un Pull Request

---

## Licencia

Este proyecto está bajo la licencia **MIT**. Ver `LICENSE` para detalles.

---

## Contacto

Para preguntas, reportar bugs o sugerir mejoras:

📧 **Email:** soporte@bluesoftbank.dev  
🐛 **Issues:** [GitHub Issues](https://github.com/your-org/Api-BluesoftBank/issues)  
💬 **Discussions:** [GitHub Discussions](https://github.com/your-org/Api-BluesoftBank/discussions)

---

## Roadmap

### ✅ Completado

- Clean Architecture + CQRS
- Seguridad transaccional
- Validación robusta
- Tests unitarios
- Documentación Swagger

### 🚀 Próximas Mejoras

- [ ] Autenticación JWT
- [ ] Rate limiting
- [ ] Caché distribuido (Redis)
- [ ] Eventos asíncrónos (RabbitMQ/Kafka)
- [ ] Monitoreo con Application Insights
- [ ] Logging centralizado (ELK Stack)

---

**Última actualización:** Abril 2026  
**Versión:** 1.0.0
