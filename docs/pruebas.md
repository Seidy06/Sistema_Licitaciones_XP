# Pruebas automatizadas

## Cobertura existente

- `Licitaciones.UnitTests`: reglas de proveedor, servicios de crear, consultar, editar y dar de baja; reglas de crear, publicar, editar y cerrar licitación (estado efectivo, protección de campos, presupuesto vs. ofertas); consulta de licitaciones (listar con filtro, detalle con mejor oferta, clasificación de ahorro y nivel de aprobación); y registro de ofertas con estado, vencimiento, duplicidad, presupuesto y monto positivo.
- `Licitaciones.IntegrationTests`: migraciones y restricciones en PostgreSQL, persistencia, Unicode, duplicidad concurrente, paginación, edición y concurrencia, baja lógica, MVC, contratos de controlador y recorridos HTTP reales mediante `WebApplicationFactory`; persistencia de crear, publicar y consultar licitación; HU-14 sobre API, FKs, CHECK e índice único de ofertas; HU-15 sobre códigos/mensajes de rechazo e inmutabilidad; HU-16 sobre selección, desempate, ausencia y clasificación de la mejor oferta; y HU-17 sobre listado, detalle, proveedor, moneda, fecha e indicador de mejor oferta.
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

## Resultado verificado para el cierre

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

Los recorridos end-to-end crean clientes sobre hosts ASP.NET Core reales. Así
verifican activación por DI, routing, model binding, serialización, vistas,
respuestas HTTP y persistencia PostgreSQL, además de las pruebas directas de
controlador ya existentes.

## Integración continua

`.github/workflows/ci.yml` se ejecuta para `push` y `pull_request` dirigidos a `main`. En Ubuntu configura .NET 9 y PostgreSQL 16, restaura, verifica formato, compila Release y ejecuta toda la solución. En esta iteración no mide cobertura ni construye imágenes Docker.
