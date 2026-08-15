# Módulo de proveedores

## Alcance terminado

HU-06 a HU-09 implementan registro, edición, baja lógica, listado y consulta de proveedores en MVC y API REST.

## Reglas reales

- El nombre admite letras, números, espacios, punto, coma y paréntesis.
- Domain aplica Unicode Form C, elimina espacios laterales, colapsa espacios repetidos y usa mayúsculas invariantes para `NombreNormalizado`.
- Los nombres persistidos tienen un máximo de 200 caracteres.
- No puede haber dos proveedores activos con el mismo nombre normalizado.
- La violación PostgreSQL `23505` del índice esperado se traduce a duplicidad.
- La edición exige la versión `xmin`; una versión desactualizada produce un conflicto de concurrencia.
- Dar de baja establece `DeletedAt`. Las consultas ordinarias solo muestran proveedores activos y un nombre dado de baja puede registrarse nuevamente.

## Componentes

| Capa | Componentes principales |
| --- | --- |
| Domain | `Proveedor`, `ProveedorNombreNormalizer`, `ProveedorNombreValidator`. |
| Application | `CrearProveedorService`, `ConsultarProveedorService`, `EditarProveedorService`, `DarBajaProveedorService`, DTO e interfaces. |
| Infrastructure | `ProveedorRepository`, configuración EF Core y migraciones. |
| API | CRUD lógico bajo `/api/v1/proveedores`. |
| Web | Listado, detalle, creación, edición y confirmación de baja. |

El histórico de proveedores dados de baja se consulta explícitamente mediante
`GET /api/v1/proveedores/historico`, su detalle por identificador y las vistas
MVC `History`/`HistoryDetails`. Infrastructure usa `IgnoreQueryFilters()` solo
en esas consultas y exige `DeletedAt != null`, por lo que el histórico no puede
mezclarse accidentalmente con el catálogo activo.
