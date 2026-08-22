# Pruebas automatizadas

## Cobertura agregada tras la auditoria final

Se agregaron dos pruebas unitarias de cierre manual y cinco recorridos HTTP
integrados: publicar, editar, cerrar, consultar licitaciones con
filtro/orden/paginacion y consultar ofertas con filtro/orden/paginacion. La
coleccion xUnit continua reutilizando una sola fixture PostgreSQL Testcontainers;
no se agregaron fixtures ni contenedores por historia.

## Cobertura existente

- `Licitaciones.UnitTests`: reglas de proveedor, servicios de crear, consultar, editar y dar de baja; reglas de crear, publicar, editar y cerrar licitación (estado efectivo, protección de campos, presupuesto vs. ofertas); consulta de licitaciones (listar con filtro, detalle con mejor oferta, clasificación de ahorro y nivel de aprobación); y registro de ofertas con estado, vencimiento, duplicidad, presupuesto y monto positivo.
- `Licitaciones.IntegrationTests`: migraciones y restricciones en PostgreSQL, persistencia, Unicode, duplicidad concurrente, paginación, edición y concurrencia, baja lógica, MVC, contratos de controlador y recorridos HTTP reales mediante `WebApplicationFactory`; persistencia de crear, publicar y consultar licitación; HU-14 sobre API, FKs, CHECK e índice único de ofertas; HU-15 sobre códigos/mensajes de rechazo e inmutabilidad; HU-16 sobre selección, desempate, ausencia y clasificación de la mejor oferta; HU-17 sobre listado, detalle, proveedor, moneda, fecha e indicador de mejor oferta; y HU-18 sobre traslapes de rangos activos, rechazo del segundo rango abierto y resolución del aprobador desde la tabla.
- `Licitaciones.FunctionalTests`: prueba funcional HTTP de la página inicial, la plantilla MVC y el formulario de crear licitación.

Las pruebas de integración usan PostgreSQL real. Si no se define `LICITACIONES_INTEGRATION_CONNECTION_STRING`, una colección compartida de xUnit inicia una sola instancia `postgres:16-alpine` para las 22 clases integradas y la elimina al terminar; esto requiere Docker en ejecución. En CI se usa el PostgreSQL 16 declarado como servicio del workflow.

## Comandos reproducibles

Desde la raíz del repositorio, con .NET SDK 9 y Docker activos:

```powershell
dotnet restore Licitaciones.sln
dotnet build Licitaciones.sln --configuration Release --no-restore
dotnet test Licitaciones.sln --configuration Release --no-build
```

Ejecución directa dejando que Testcontainers cree PostgreSQL:

```powershell
dotnet test Licitaciones.sln --configuration Release
```

Ejecución contra el PostgreSQL de Compose:

```powershell
docker compose up -d postgres
$env:LICITACIONES_INTEGRATION_CONNECTION_STRING = "Host=127.0.0.1;Port=5432;Database=licitaciones_db;Username=licitaciones_user;Password=licitaciones_password"
dotnet test Licitaciones.sln --configuration Release
Remove-Item Env:LICITACIONES_INTEGRATION_CONNECTION_STRING
```

## Resultado verificado para el cierre de la Iteración 2

Ejecución local del 21 de agosto de 2026, después del refactor de HU-17 y con
PostgreSQL real iniciado por Testcontainers:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 81 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 91 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 3 | 0 | 0 |
| **Total ejecutado** | **175** | **0** | **0** |

Para HU-14, la cobertura específica incluye 8 casos unitarios del servicio, 6
casos HTTP y 7 casos de persistencia (contando por separado los datos de las
teorías). La suite completa se ejecutó antes y después del refactor.

Para HU-15 se agregaron cinco casos HTTP integrados: duplicidad (`409`), exceso
de presupuesto (`422`), vencimiento (`422`), intento de edición y de eliminación
en una licitación cerrada (`422`). Los dos últimos vuelven a consultar
PostgreSQL y comprueban que licitación, proveedor y monto permanecen intactos.

Para HU-16 se agregaron cinco casos de Application y cinco casos HTTP
integrados. Cubren menor monto, desempate por `FechaRegistro`, mensaje sin
ofertas, ahorro exactamente igual a 10 %, ahorro entre 0 % y 10 %, y oferta
igual al presupuesto. Las cinco pruebas HTTP usan PostgreSQL real mediante
Testcontainers.

Para HU-17 se agregaron dos casos HTTP integrados. El listado comprueba
proveedor, monto CRC, fecha de registro y selección de la mejor oferta; el
detalle solicita USD y comprueba la conversión mediante el tipo de cambio activo
sin modificar el monto almacenado. Ambas pruebas recorren API, Application,
Infrastructure y PostgreSQL real.

## Resultado verificado para HU-18 (Iteración 3)

Ejecución local del 22 de agosto de 2026 con `dotnet test Licitaciones.sln`,
después del refactor de HU-18 y con PostgreSQL real iniciado por Testcontainers:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 83 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 101 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 3 | 0 | 0 |
| **Total ejecutado** | **187** | **0** | **0** |

HU-18 aporta cinco pruebas de integración sobre PostgreSQL real, todas con el
trait `HU-18`. Dos (`NivelAprobacionOverlapPersistenceTests`) comprueban dentro
de una transacción revertida que la restricción de exclusión rechaza un segundo
rango traslapado y un segundo rango abierto con el error `23P01`. Tres
(`NivelAprobacionAdminHttpTests`) verifican por HTTP real que el traslape
responde `409 Conflict` sin persistir el segundo nivel, que un segundo rango
abierto se rechaza con `409`, y que la resolución consulta realmente la tabla:
inserta un nivel especial, resuelve un monto dentro de su rango y restaura el
catálogo sembrado.

La suite completa se ejecutó antes y después del refactor. En CI, el commit
rojo `bd1f3d6` falló como es esperable en TDD (ejecución `32532418505`), el
verde `249ab70` terminó en `success` (ejecución `32534822110`) y el refactor
`1224ece` terminó en `success` (ejecución `32556366636`).

Los recorridos end-to-end crean clientes sobre hosts ASP.NET Core reales. Así
verifican activación por DI, routing, model binding, serialización, vistas,
respuestas HTTP y persistencia PostgreSQL, además de las pruebas directas de
controlador ya existentes.

## Integración continua

`.github/workflows/ci.yml` se ejecuta para `push` y `pull_request` dirigidos a `main`. En Ubuntu configura .NET 9 y PostgreSQL 16, restaura, verifica formato, compila Release y ejecuta toda la solución. En esta iteración no mide cobertura ni construye imágenes Docker.

Estado verificado en la API pública de GitHub al cierre de la Iteración 2 (20
de agosto de 2026): los ocho commits de fusión del incremento terminaron en
`success` en `main` — `cccfa2d` (PR #19, ejecución `32217608694`), `0fc34be`
(PR #20, `32258741686`), `cbd8fed` (PR #21, `32285499762`), `1f5c453`
(PR #22, `32318996472`), `d154284` (PR #23, `32337758472`), `370e1ac`
(PR #24, `32383569802`), `0be6570` (PR #25, `32397368491`) y `9966565`
(PR #26, `32450135648`). Cada ejecución incluye el paso
`dotnet format --verify-no-changes`, por lo que el criterio de formato del DoD
queda cubierto por CI. Los commits rojo de las ramas fallaron como era
esperable dentro del ciclo TDD y quedaron en verde tras la implementación; el
único fallo no funcional fue el orden de imports de `fc87fe0`, corregido en
`7b49708`.
