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
