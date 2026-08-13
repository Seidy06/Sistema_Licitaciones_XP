# Bitácora XP

## Iteración 1 - Base y proveedores

### HU-01 - Registrar proveedor

**Issue:** [#8](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/8)

**Pull Request:** [#9](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/9)

**Rama:** `hu/01-proveedores`

### Ciclo TDD

1. Rojo
   - Se escribieron pruebas de normalización de espacios, mayúsculas/minúsculas
     y Unicode, caracteres permitidos, creación y duplicidad.
   - Se agregaron pruebas de integración para persistencia e índice único en
     PostgreSQL.
   - Las pruebas fallaron antes de existir la implementación necesaria.

2. Verde
   - Se implementaron `ProveedorNombreNormalizer`,
     `ProveedorNombreValidator`, la entidad `Proveedor` y
     `CrearProveedorService` con el comportamiento mínimo requerido.
   - Se añadió el repositorio de Entity Framework Core, la migración, el
     endpoint REST y el formulario MVC.
   - Las pruebas pasaron satisfactoriamente.

3. Refactorización
   - Se centralizó la normalización en `ProveedorNombreNormalizer`.
   - El controlador MVC quedó limitado a coordinar HTTP y delegar las reglas en
     `CrearProveedorService` y el dominio.
   - Se mantuvo el comportamiento observable durante la refactorización.

### Persistencia

La unicidad del proveedor se valida tanto en Application como en PostgreSQL
mediante el índice único `UX_Proveedores_NombreNormalizado`. La aplicación Web
aplica las migraciones pendientes al iniciar antes de aceptar solicitudes.

### Driver / Navigator

| Actividad registrada | Driver | Navigator |
| --- | --- | --- |
| Interfaz MVC de proveedores | Persona B | Persona A |

Esta es la rotación confirmada para la actividad documentada. Las rotaciones de
sesiones anteriores no se infieren cuando no existe una evidencia registrada.

### Evidencias

- [Issue #8](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/8).
- [Pull Request #9](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/9).
- Commits principales: `f274b20` (normalización), `f597141` (servicio y
  duplicidad), `0fd4129` (persistencia), `89f0768` (API) y `1516d2c` (MVC).
- Validación local: `dotnet build Licitaciones.sln` sin advertencias ni errores.
- Pruebas locales: 18 unitarias, 3 de integración y 1 funcional; 22 superadas.
- GitHub Actions: el workflow `.github/workflows/ci.yml` restaura, compila y
  prueba la solución con PostgreSQL 16 para cada pull request dirigido a
  `main`. El resultado de la ejecución se consulta en los checks del
  [PR #9](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/9/checks).

## Actualización de planificación posterior a la auditoría de la Iteración 1

Después de la auditoría de la Iteración 1, el equipo adoptó formalmente el
catálogo HU-00 a HU-37 y reorganizó la planificación en cuatro iteraciones:
HU-00 a HU-09, HU-10 a HU-17, HU-18 a HU-27 y HU-28 a HU-37.

La evidencia auditada de proveedores conserva la numeración que estaba vigente
cuando se produjo. Para continuar el proyecto se aplica esta equivalencia:

| Historia auditada anteriormente | Historia del catálogo actual |
| --- | --- |
| HU-01 — Registrar proveedor | HU-06 — Registrar proveedor |
| HU-02 — Consultar proveedores | HU-09 — Listar y consultar proveedores |
| HU-03 — Editar proveedor | HU-07 — Editar proveedor |
| HU-04 — Eliminar proveedor | HU-08 — Eliminar lógicamente proveedor |

Los commits históricos, incluidos los citados en esta bitácora, no se
modificarán ni se reescribirán porque constituyen evidencia XP del repositorio.
La equivalencia anterior permite relacionarlos con el catálogo actual sin
alterar el historial.

## HU-06 - Validación Unicode y duplicidad concurrente

Después de adoptar el catálogo actual se amplió la evidencia de registro de
proveedores en el [Pull Request
#12](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/12).

### Ciclo TDD

1. Rojo — `1666a8d`
   - Se agregaron pruebas para Unicode compuesto y descompuesto de extremo a
     extremo.
   - Se reprodujo una carrera en la que dos solicitudes equivalentes superan
     la consulta previa de duplicidad.
   - Se exigió que el contrato HTTP produzca `201 Created` y `409 Conflict`, sin
     propagar un error `500`.

2. Verde — `23aa497`
   - Domain centralizó Unicode Form C, espacios y el valor comparable.
   - Infrastructure capturó específicamente la violación PostgreSQL `23505` de
     `UX_Proveedores_NombreNormalizado` y la tradujo a
     `ProveedorDuplicadoException`.
   - MVC y API conservaron controladores delgados y delegaron las reglas al
     dominio y a Application.

3. Refactorización — `276d9af`
   - `ProveedorNombreNormalizer` pasó a producir conjuntamente el nombre
     legible y `NombreNormalizado`, evitando cálculos repetidos.
   - El nombre del índice único se centralizó en la configuración de
     persistencia para compartirlo con la traducción del conflicto.
   - El comportamiento permaneció verde: 25 pruebas unitarias, 11 de
     integración y 1 funcional, para un total de 37 pruebas superadas.
