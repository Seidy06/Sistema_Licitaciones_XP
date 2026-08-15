# HU-09: Listar y consultar proveedores

## Alcance

HU-09 incorpora la consulta de proveedores por identificador y un listado con
paginación, filtro por nombre y ordenamiento. Application devuelve DTOs y nunca
expone directamente entidades administradas por Entity Framework Core.

## Contrato de consulta

El listado acepta:

| Parámetro | Predeterminado | Regla |
| --- | --- | --- |
| `pagina` | `1` | Debe ser mayor o igual que 1. |
| `tamanoPagina` | `20` | Debe estar entre 1 y 100. |
| `nombre` | Sin filtro | Coincidencia parcial case-insensitive. |
| `ordenarPor` | `Nombre` | Admite `Nombre` o `FechaCreacion`. |
| `descendente` | `false` | Invierte el sentido del orden. |

`PaginaResultado<ProveedorDto>` contiene los elementos solicitados, el total de
coincidencias, la página actual y el tamaño de página. Infrastructure aplica el
filtro, el orden y `Skip/Take` en PostgreSQL.

La consulta por Id devuelve un `ProveedorDto` cuando el proveedor existe y
`null` desde Application cuando no existe. API y MVC traducen ese resultado al
contrato propio de cada interfaz.

## API REST

### Listado

`GET /api/v1/proveedores`

Ejemplo:

```text
GET /api/v1/proveedores?pagina=2&tamanoPagina=10&nombre=central&ordenarPor=FechaCreacion&descendente=true
```

La respuesta exitosa es `200 OK` con una página de DTOs:

```json
{
  "items": [],
  "total": 0,
  "pagina": 2,
  "tamanoPagina": 10
}
```

### Detalle

`GET /api/v1/proveedores/{id}`

- `200 OK`: devuelve el `ProveedorDto` encontrado.
- `404 Not Found`: no existe un proveedor activo con el Id indicado.

## MVC

- `GET /Proveedores`: muestra el listado, filtro, ordenamiento y navegación de
  páginas.
- `GET /Proveedores/Details/{id}`: muestra el detalle del proveedor.
- Un Id inexistente en el detalle produce `NotFound`.

Los controladores MVC proyectan los DTOs a
`ProveedorResumenViewModel` y `ProveedorDetalleViewModel`.

## Borrado lógico

HU-08 implementa `DeletedAt` y un filtro global de EF Core. Las consultas
activas de HU-09 heredan ese filtro y no incluyen proveedores eliminados. El
histórico se expone únicamente mediante las rutas explícitas de HU-08; esas
consultas usan `IgnoreQueryFilters()` y filtran filas con `DeletedAt`.

## Evidencia TDD

### ROJO — `01f2499`

- Pruebas unitarias para consulta por Id, resultado inexistente y coordinación
  de la consulta paginada.
- Pruebas de integración para filtro case-insensitive, paginación y orden por
  nombre o fecha de creación.
- Contratos HTTP 200/404 y acciones MVC de listado y detalle.
- La ejecución falló por los contratos y métodos de consulta todavía ausentes.

### VERDE — `334e618`

- Se incorporaron `ConsultarProveedorService`, el repositorio de consulta y los
  modelos de paginación.
- `ProveedorRepository` implementó las consultas sobre PostgreSQL.
- API y MVC incorporaron los endpoints, acciones y vistas requeridos.
- Las respuestas públicas se mantuvieron separadas de las entidades EF.

## Verificación

```powershell
dotnet build Licitaciones.sln --configuration Release
dotnet test Licitaciones.sln --configuration Release
git diff --check
git status
```
