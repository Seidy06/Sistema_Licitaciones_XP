# Pruebas

## Estrategia para HU-01

### Unitarias

- Normalización de espacios laterales y repetidos.
- Comparación sin distinguir mayúsculas y minúsculas.
- equivalencia de Unicode compuesto y descompuesto.
- Aceptación y rechazo de caracteres.
- Creación válida de la entidad.
- Coordinación del servicio y rechazo de duplicados.

### Integración

Las pruebas usan PostgreSQL real mediante Testcontainers y aplican las
migraciones del proyecto. Verifican:

- persistencia y recuperación del proveedor;
- rechazo de nombres normalizados duplicados;
- existencia y naturaleza única de
  `UX_Proveedores_NombreNormalizado`.

### Comandos

```powershell
dotnet build Licitaciones.sln
dotnet test Licitaciones.sln
```

Resultado local registrado para HU-01:

| Proyecto | Superadas | Fallidas |
| --- | ---: | ---: |
| `Licitaciones.UnitTests` | 18 | 0 |
| `Licitaciones.IntegrationTests` | 3 | 0 |
| `Licitaciones.FunctionalTests` | 1 | 0 |
| **Total** | **22** | **0** |

También se verificó manualmente el formulario mediante solicitudes HTTP con
cookie y token antifalsificación para los casos válido, duplicado e inválido.

## Integración continua

El workflow `ci.yml` ejecuta restore, build Release y pruebas en Ubuntu con un
servicio PostgreSQL 16. La evidencia de la ejecución asociada se conserva en
los checks del [Pull Request
#9](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/9/checks).
