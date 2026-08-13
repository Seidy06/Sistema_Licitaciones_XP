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
producen `EMPRESA CENTRAL`. También son equivalentes `Café Central` y su forma
descompuesta `Cafe\u0301 Central`, porque Domain aplica Unicode Form C antes de
guardar. Infrastructure reconoce exclusivamente la violación PostgreSQL
`23505` de este índice y la traduce al conflicto de proveedor duplicado.

### Migración

La migración `20260810005236_CreateProviders` crea la tabla y el índice. La Web
ejecuta `Database.MigrateAsync()` al iniciar para aplicar migraciones pendientes
antes de atender solicitudes.

## Licitaciones y estados

`EstadosLicitacion` contiene los cinco estados parametrizados: Borrador,
Publicada, Cerrada, Adjudicada y Cancelada. `Licitaciones` referencia ese
catálogo mediante llave foránea y almacena presupuesto como `numeric(18,2)`,
con `CHECK` positivo, fecha de cierre y marcas de tiempo en UTC.

## Ofertas

`Ofertas` referencia a `Licitaciones` y `Proveedores` mediante llaves foráneas
restrictivas. El monto usa `numeric(18,2)` y tiene un `CHECK` que exige un valor
positivo. El índice único compuesto por licitación y proveedor evita más de una
oferta del mismo proveedor en una licitación.

## Niveles de aprobación

`NivelesAprobacion` usa límites `numeric(18,2)`. La restricción de exclusión
`EX_NivelesAprobacion_SinTraslape` impide rangos superpuestos. La semilla inicial
contiene los niveles Operativo, Gerencial y Directivo con intervalos contiguos.

## Tipos de cambio

`TiposCambio` guarda el valor como `numeric(18,6)`, exige que sea positivo y
mantiene un único registro activo mediante el índice parcial
`UX_TiposCambio_Activo`. La semilla inicial registra USD/CRC activo.

## Auditoría temporal

Todas las entidades auditables incluyen `CreatedAt` y `UpdatedAt`. El
`LicitacionesDbContext` asigna ambas marcas automáticamente al insertar y
actualiza `UpdatedAt` al modificar, usando el servicio `IClock` inyectado.

La migración `CompleteInitialDomain` es la fuente ejecutable de tablas,
relaciones, semillas y restricciones de esta primera versión completa.
