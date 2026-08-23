# Pruebas automatizadas

## Cobertura agregada tras la auditoria final

Se agregaron dos pruebas unitarias de cierre manual y cinco recorridos HTTP
integrados: publicar, editar, cerrar, consultar licitaciones con
filtro/orden/paginacion y consultar ofertas con filtro/orden/paginacion. La
coleccion xUnit continua reutilizando una sola fixture PostgreSQL Testcontainers;
no se agregaron fixtures ni contenedores por historia.

## Cobertura existente

- `Licitaciones.UnitTests`: reglas de proveedor, servicios de crear, consultar, editar y dar de baja; reglas de crear, publicar, editar y cerrar licitación (estado efectivo, protección de campos, presupuesto vs. ofertas); consulta de licitaciones (listar con filtro, detalle con mejor oferta, clasificación de ahorro y nivel de aprobación); y registro de ofertas con estado, vencimiento, duplicidad, presupuesto y monto positivo.
- `Licitaciones.IntegrationTests`: migraciones y restricciones en PostgreSQL, persistencia, Unicode, duplicidad concurrente, paginación, edición y concurrencia, baja lógica, MVC, contratos de controlador y recorridos HTTP reales mediante `WebApplicationFactory`; persistencia de crear, publicar y consultar licitación; HU-14 sobre API, FKs, CHECK e índice único de ofertas; HU-15 sobre códigos/mensajes de rechazo e inmutabilidad; HU-16 sobre selección, desempate, ausencia y clasificación de la mejor oferta; HU-17 sobre listado, detalle, proveedor, moneda, fecha e indicador de mejor oferta; HU-18 sobre traslapes de rangos activos, rechazo del segundo rango abierto y resolución del aprobador desde la tabla; y HU-19 sobre reemplazo del tipo de cambio activo, conversión USD sin modificar montos persistidos, fecha del tipo de cambio utilizado y operación sin conexión externa.
- `Licitaciones.FunctionalTests`: prueba funcional HTTP de la página inicial, la plantilla MVC, el formulario de crear licitación; HU-20 sobre la landing informativa en la raíz `/`: acceso anónimo con las seis secciones explicativas y diseño responsivo (viewport, Bootstrap y rejilla por puntos de ruptura) simulando un agente móvil; y HU-21 sobre la navegación global y el acceso a Swagger.

Las pruebas de integración usan PostgreSQL real. Si no se define `LICITACIONES_INTEGRATION_CONNECTION_STRING`, una colección compartida de xUnit inicia una sola instancia `postgres:16-alpine` para las 22 clases integradas y la elimina al terminar; esto requiere Docker en ejecución. En CI se usa el PostgreSQL 16 declarado como servicio del workflow.

## Resultado verificado para HU-21 (Iteración 3)

Ejecución local del 22 de agosto de 2026 con `dotnet test Licitaciones.sln`,
después del refactor y con PostgreSQL real iniciado por Testcontainers:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 83 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 105 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 11 | 0 | 0 |
| **Total ejecutado** | **199** | **0** | **0** |

HU-21 aporta seis pruebas funcionales con trait `HU-21` en
`NavegacionGlobalWebTests`: presencia del menú global en las páginas cubiertas,
resaltado de Inicio, traslado del resaltado a Licitaciones y apertura de
Swagger UI. La ejecución filtrada de HU-21 terminó con 6 correctas, 0 fallidas
y 0 omitidas; la suite completa también se ejecutó antes y después del
refactor.

Los commits `a9dd711` (ROJO), `0e226e7` (VERDE), `b206f9c` (ajuste de estilo)
y `e2fbd06` (REFACTOR) no tienen ejecución de CI asociada a un PR de HU-21 en
esta fase.

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

## Resultado verificado para HU-19 (Iteración 3)

Ejecución local del 22 de agosto de 2026 con `dotnet test Licitaciones.sln
--configuration Release --no-build`, antes y después del refactor, con
PostgreSQL real iniciado por Testcontainers:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 83 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 105 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 3 | 0 | 0 |
| **Total ejecutado** | **191** | **0** | **0** |

HU-19 aporta cuatro pruebas de integración sobre PostgreSQL real, todas con el
trait `HU-19` y recorridos HTTP reales mediante `WebApplicationFactory`:

1. Guardar un nuevo tipo de cambio activo desactiva el previo y deja un único
   registro activo; la comprobación consulta directamente la tabla y verifica
   que la semilla quedó inactiva.
2. Tras guardar una tasa nueva, el detalle de oferta en USD divide el monto
   entre el nuevo valor sin modificar el monto persistido en CRC.
3. La vista en USD incluye el valor y la fecha del tipo de cambio utilizado
   (`tipoCambioValor` y `tipoCambioFecha`).
4. Bloqueando toda llamada HTTP saliente, tanto la consulta del activo como
   la conversión funcionan con el tipo de cambio administrado localmente.

Cada prueba restaura el tipo de cambio semilla (USD→CRC, valor 500,
2026-01-01) para no contaminar los escenarios restantes.

En CI, el commit rojo `78be404` falló como es esperable en TDD (ejecución
`32579740531`) y el verde `7ab8e09` terminó en `success` (ejecución
`32597593088`). El commit de refactor `ff92f44` permanece local a la espera de
publicarse en el PR #59: su verificación fue íntegramente local (build Release
sin errores ni advertencias, `dotnet format --verify-no-changes` sin
diferencias y suite completa en verde antes y después), por lo que todavía no
tiene ejecución de CI registrada.

Los recorridos end-to-end crean clientes sobre hosts ASP.NET Core reales. Así
verifican activación por DI, routing, model binding, serialización, vistas,
respuestas HTTP y persistencia PostgreSQL, además de las pruebas directas de
controlador ya existentes.

## Resultado verificado para HU-20 (Iteración 3)

Ejecución local del 22 de agosto de 2026 con `dotnet test Licitaciones.sln`,
después del ciclo de HU-20 y con PostgreSQL real iniciado por Testcontainers:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 83 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 105 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 5 | 0 | 0 |
| **Total ejecutado** | **193** | **0** | **0** |

HU-20 aporta dos pruebas funcionales HTTP reales mediante
`WebApplicationFactory`, ambas con el trait `HU-20` y sin tocar persistencia:

1. `Raiz_SinAutenticacion_DebeMostrarLandingConSeccionesExplicativas`: un
   cliente anónimo obtiene `200 OK` en `/` sin redirección a autenticación y
   el HTML contiene, sin distinguir mayúsculas, las seis secciones esperadas:
   propósito de la aplicación, flujo de licitación, ofertas, mejor oferta,
   nivel de aprobación y conversión monetaria.
2. `Landing_ConDispositivoMovil_DebeSerResponsiva`: con un agente móvil,
   verifica la meta viewport con `width=device-width`, la hoja de estilos de
   Bootstrap, el cuerpo renderizado dentro de `<main>` y al menos tres clases
   de columna por punto de ruptura en el contenido principal.

La prueba funcional previa de la plantilla (`PlantillaWebTests`) conservó sus
aserciones; solo ajustó su trait a `HU-00`. El ciclo de esta historia quedó
publicado en un único commit que combina pruebas e implementación mínima, por
lo que no existe ejecución de CI en rojo registrada para HU-20 (desviación de
proceso anotada en la bitácora). En CI, el commit `8062619` terminó en
`success` (ejecución `32613192010`) como parte del PR #60. El refactor se
evaluó y se rechazó sin cambios de código: extraer los bloques repetidos a
expresiones Razor rompe los caracteres acentuados porque `HtmlEncoder.Default`
escapa todo lo que está fuera de Basic Latin, y las alternativas (`Html.Raw`
o reconfigurar el encoder global) son peores que la duplicación idiomática de
una página estática; la suite permaneció verde antes y después de esa
verificación local.

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
