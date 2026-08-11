# Modelo de datos

## Proveedores

Tabla: `Proveedores`.

| Columna | Tipo PostgreSQL | Restricción |
| --- | --- | --- |
| `Id` | `uuid` | Clave primaria; generado por el dominio. |
| `Nombre` | `varchar(200)` | Obligatorio; representación legible. |
| `NombreNormalizado` | `varchar(200)` | Obligatorio; valor comparable y único. |
| `CreatedAt` | `timestamp with time zone` | Obligatorio; fecha UTC de creación. |
| `UpdatedAt` | `timestamp with time zone` | Obligatorio; fecha UTC de actualización. |
| `xmin` | `xid` | Versión de fila administrada por PostgreSQL. |

### Unicidad

El índice único `UX_Proveedores_NombreNormalizado` impide duplicados incluso si
dos operaciones concurrentes superan la comprobación previa de Application.
Por ejemplo, `Empresa Central`, `empresa central` y `  EMPRESA   CENTRAL  `
producen `EMPRESA CENTRAL`.

### Migración

La migración `20260810005236_CreateProviders` crea la tabla y el índice. La Web
ejecuta `Database.MigrateAsync()` al iniciar para aplicar migraciones pendientes
antes de atender solicitudes.
