# Pruebas automatizadas

## Cobertura agregada tras la auditoria final

Se agregaron dos pruebas unitarias de cierre manual y cinco recorridos HTTP
integrados: publicar, editar, cerrar, consultar licitaciones con
filtro/orden/paginacion y consultar ofertas con filtro/orden/paginacion. La
coleccion xUnit continua reutilizando una sola fixture PostgreSQL Testcontainers;
no se agregaron fixtures ni contenedores por historia.

## Cobertura existente

- `Licitaciones.UnitTests`: reglas de proveedor, servicios de crear, consultar, editar y dar de baja; reglas de crear, publicar, editar y cerrar licitación (estado efectivo, protección de campos, presupuesto vs. ofertas); consulta de licitaciones (listar con filtro, detalle con mejor oferta, clasificación de ahorro y nivel de aprobación); y registro de ofertas con estado, vencimiento, duplicidad, presupuesto y monto positivo. Desde HU-28 también: validación y monedas predeterminadas del tipo de cambio, administración del tipo de cambio activo con orden/paginación, validaciones y desactivación de niveles de aprobación, administración de niveles (traslape, filtro, orden, desactivación) y consulta de ofertas con conversión CRC/USD, filtro, orden, paginación e indicador de mejor oferta.
- `Licitaciones.IntegrationTests`: migraciones y restricciones en PostgreSQL, persistencia, Unicode, duplicidad concurrente, paginación, edición y concurrencia, baja lógica, MVC, contratos de controlador y recorridos HTTP reales mediante `WebApplicationFactory`; persistencia de crear, publicar y consultar licitación; HU-14 sobre API, FKs, CHECK e índice único de ofertas; HU-15 sobre códigos/mensajes de rechazo e inmutabilidad; HU-16 sobre selección, desempate, ausencia y clasificación de la mejor oferta; HU-17 sobre listado, detalle, proveedor, moneda, fecha e indicador de mejor oferta; HU-18 sobre traslapes de rangos activos, rechazo del segundo rango abierto y resolución del aprobador desde la tabla; y HU-19 sobre reemplazo del tipo de cambio activo, conversión USD sin modificar montos persistidos, fecha del tipo de cambio utilizado y operación sin conexión externa.
- `Licitaciones.FunctionalTests`: prueba funcional HTTP de la página inicial, la plantilla MVC, el formulario de crear licitación; HU-20 sobre la landing informativa en la raíz `/`: acceso anónimo con las seis secciones explicativas y diseño responsivo (viewport, Bootstrap y rejilla por puntos de ruptura) simulando un agente móvil; HU-21 sobre la navegación global y el acceso a Swagger; y HU-22 sobre modo claro/oscuro persistente: control visible en todas las páginas, persistencia de la preferencia en `localStorage` y respeto del último tema seleccionado al cargar.

Las pruebas de integración usan PostgreSQL real. Si no se define `LICITACIONES_INTEGRATION_CONNECTION_STRING`, una colección compartida de xUnit inicia una sola instancia `postgres:16-alpine` para las 22 clases integradas y la elimina al terminar; esto requiere Docker en ejecución. En CI se usa el PostgreSQL 16 declarado como servicio del workflow.

## Resultado verificado para HU-23 (Iteración 3)

HU-23 corresponde a la Issue [#52](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/52).
Sus tres criterios se cubren mediante pruebas de integración HTTP reales con
`WebApplicationFactory`, vistas MVC y PostgreSQL mediante Testcontainers:

| Criterio de aceptación de la Issue #52 | Pruebas | Evidencia |
| --- | --- | --- |
| Cada listado soporta paginación, filtrado y ordenamiento. | `Listado_Proveedores_DebeSoportarPaginacionFiltroYOrden`, `Listado_Licitaciones_DebeSoportarPaginacionFiltroYOrden`, `Listado_Ofertas_DebeSoportarPaginacionFiltroYOrden`, `Listado_NivelesAprobacion_DebeSoportarPaginacionFiltroYOrden` y `Listado_TiposCambio_DebeSoportarPaginacionYOrden` en `CrudWebListadosTests`. | Comprueban tablas HTML, filtros, páginas sucesivas y orden descendente cuando aplica. |
| Los formularios inválidos muestran validación junto al campo y conservan los datos ingresados. | `Formulario_Proveedores_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos`, `Formulario_Licitaciones_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos`, `Formulario_Ofertas_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos`, `Formulario_NivelesAprobacion_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos` y `Formulario_TiposCambio_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos` en `CrudWebFormulariosInvalidosTests`. | Comprueban respuesta sin redirección, mensaje de validación asociado y valores enviados conservados. |
| Toda eliminación permitida solicita confirmación antes de ejecutarse. | `Eliminacion_Proveedores_DebePedirConfirmacionAntesDeEjecutar` y `Eliminacion_NivelesAprobacion_DebePedirConfirmacionAntesDeEjecutar` en `CrudWebConfirmacionEliminacionTests`. | Comprueban la vista y formulario de confirmación, el estado intacto antes del POST y la baja lógica o desactivación posterior. |

La ejecución focalizada de las tres clases terminó con **12 pruebas correctas,
0 fallidas y 0 omitidas**. La suite completa posterior al refactor, ejecutada
con `dotnet test Licitaciones.sln`, terminó con **216 correctas, 0 fallidas y
0 omitidas**. El build reportó dos advertencias `CS1998` preexistentes en
pruebas funcionales; no están relacionadas con HU-23.

La secuencia TDD y el refactor quedan trazados así:

- `b5ff1fe` — ROJO: agregó las pruebas de los tres criterios.
- `e4b7973` — VERDE: implementó el CRUD MVC para los módulos cubiertos.
- `5b3be34` — corrección de imports para CI, sin ampliar el alcance.
- `2803c00` — REFACTOR local: movió `PaginaResultado<T>` a
   `Licitaciones.Application.Common`, sin comportamiento nuevo.

El PR [#63](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/63) está
abierto y mergeable hacia `main`. Los commits hasta `5b3be34` están publicados;
`2803c00` permanece local y no tiene ejecución de CI registrada. La Issue #52
permanece abierta y no se marca como completada desde esta fase.

## Resultado verificado para HU-24 (Iteración 3)

HU-24 corresponde a la Issue [#53](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/53).
Sus dos criterios se cubren mediante pruebas de integración HTTP reales con
`WebApplicationFactory`, vistas MVC y PostgreSQL mediante Testcontainers:

| Criterio de aceptación de la Issue #53 | Pruebas | Evidencia |
| --- | --- | --- |
| Una operación exitosa muestra un mensaje de confirmación (toast/alert). | `Operacion_Exitosa_EliminacionNivel_DebeMostrarAlertaConfirmacionEnDestino` y `Operacion_Exitosa_RegistroOferta_DebeMostrarAlertaConfirmacionEnListado` en `MensajeriaWebTests`. | Siguen la redirección del POST y comprueban que la página destino presenta `alert-success` con el mensaje de confirmación. |
| Un error de negocio produce un mensaje específico y comprensible, sin stack trace. | `ErrorNegocio_TraslapeNiveles_DebeMostrarAlertaConMensajeEspecificoSinStacktrace` en `MensajeriaWebTests`. | Comprueba `alert-danger`, el mensaje específico de traslape de rangos y la ausencia de stack traces en la respuesta. |

La ejecución focalizada con
`dotnet test tests\Licitaciones.IntegrationTests\Licitaciones.IntegrationTests.csproj --filter "HU=HU-24"`
terminó en ROJO con **3 fallidas y 0 correctas** (comportamiento ausente) y tras
el VERDE con **3 correctas, 0 fallidas y 0 omitidas**. La suite completa
ejecutada con `dotnet test Licitaciones.sln` pasó de **216** a **219 correctas,
0 fallidas y 0 omitidas**, y se mantuvo en 219 después del refactor.

La secuencia TDD queda trazada así:

- `6b15bed` — ROJO: agregó las tres pruebas de mensajería; CI fallido como es
  esperable en rojo.
- `e6213df` — VERDE: parcial compartido `_Mensajes` con alertas de éxito y
  error incluido en las vistas destino; CI en verde.
- REFACTOR local sin commit: extendió el parcial a las vistas restantes con
  bloques duplicados, limpió los usings de `Program.cs` y ajustó la prueba
  estructural `CreateView_DebeRenderizarMensajesDeValidacionDelNombre` al nuevo
  formato sin reducir su intención.

El PR [#64](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/64)
(`iteracion-3/hu-24-mensajeria` hacia `main`) está abierto como draft. El
refactor permanece local, sin push ni CI registrado. La Issue #53 permanece
abierta y no se marca como completada desde esta fase.

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

## Resultado verificado para HU-22 (Iteración 3)

Ejecuciones locales del 23 de agosto de 2026 con PostgreSQL real iniciado por
Testcontainers. La fase ROJO se confirmó con la ejecución filtrada
`dotnet test tests\Licitaciones.FunctionalTests\Licitaciones.FunctionalTests.csproj --filter "HU=HU-22"`:
5 fallidas y 0 superadas, todas por aserciones de comportamiento ausente (sin
control `theme-toggle`, sin lógica de `localStorage`, sin paleta oscura), no por
errores artificiales. Tras el VERDE (`9975a05`) la misma ejecución filtrada
terminó con 5 correctas, 0 fallidas y 0 omitidas. Después del refactor local,
la suite completa `dotnet test Licitaciones.sln` resultó:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 83 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 105 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 16 | 0 | 0 |
| **Total ejecutado** | **204** | **0** | **0** |

HU-22 aporta cinco casos funcionales con trait `HU-22` en
`TemaClaroOscuroWebTests`, todos HTTP mediante `WebApplicationFactory` sin
persistencia: presencia del control `theme-toggle` accesible en el encabezado de
las tres páginas cubiertas; persistencia de la preferencia mediante
`localStorage.setItem('theme', …)` con valores `light`/`dark` en `site.js`; y
respeto del último tema seleccionado mediante un script inicial que lee
`localStorage.getItem('theme')` sobre el elemento raíz más la paleta oscura
definida en `site.css`. Los commits del ciclo son `12aa5c4` (ROJO) y `9975a05`
(VERDE), ambos incluidos en el PR #62 con CI en verde; los cambios de refactor
permanecen locales sin commit ni ejecución de CI registrada.

## Resultado verificado para HU-25 (Iteración 3)

Ejecuciones locales del 23 de agosto de 2026 con PostgreSQL real iniciado por
Testcontainers. La fase ROJO se confirmó con la ejecución filtrada
`dotnet test tests\Licitaciones.IntegrationTests\Licitaciones.IntegrationTests.csproj --filter "HU=HU-25"`:
3 fallidas y 0 superadas, todas porque las vistas renderizaban los montos con
`.ToString("N2")` sin cultura es-CR ni símbolo colón, no por errores
artificiales. Tras el VERDE (`f3ff76e`) la misma ejecución filtrada terminó con
3 correctas, 0 fallidas y 0 omitidas. Después del refactor (`4fd4175`), la suite
completa `dotnet test Licitaciones.sln` resultó:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 83 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 123 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 16 | 0 | 0 |
| **Total ejecutado** | **222** | **0** | **0** |

HU-25 aporta tres casos de integración HTTP con trait `HU-25` en
`FormatoMonetarioWebTests`, sobre PostgreSQL real y vistas MVC servidas por
`WebApplicationFactory`: el listado de licitaciones presenta el presupuesto
sembrado como `₡1.500.000,00`; el listado de ofertas presenta el monto de una
oferta registrada vía servicios como `₡1.250.500,00`; y el listado de niveles de
aprobación presenta `₡23.456.789,00` y `₡24.654.321,00` para un nivel sembrado
con rango único (desactivando temporalmente el nivel Directivo del catálogo y
restaurándolo al final). El ciclo quedó trazado así: `857f458` (ROJO) y
`f3ff76e` (VERDE) están publicados en la rama del PR #65; en CI el rojo falló
como es esperable (ejecución `32669889839`) y el verde terminó en `success`
(ejecución `32673569441`). El refactor `4fd4175` solo ajusta legibilidad de las
pruebas (literal ₡ e import de `Domain.Aprobaciones`), permanece local sin CI
registrada, y el código de producción se evaluó sin cambios.

## Resultado verificado para HU-26 (Iteración 3)

HU-26 corresponde a la Issue [#55](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/55).
Los cinco criterios se cubren con las pruebas HTTP previas de los módulos
(DTOs, rutas `/api/v1`, listados con paginación/filtrado/orden) más una clase
nueva para el contrato transversal de errores:

| Criterio de aceptación de la Issue #55 | Pruebas | Evidencia |
| --- | --- | --- |
| Cualquier endpoint retorna DTOs específicos, nunca entidades EF Core. | `ProveedorConsultaHttpTests`, `ConsultarLicitacionHttpTests`, `CrearOfertaHttpTests` y el resto de la suite HTTP previa. | Los controladores declaran `[ProducesResponseType]` tipado y las aserciones existentes validan los contratos. |
| La ruta base incluye versión (`/api/v1/...`). | Rutas verificadas en los cinco controladores API. | `api/v1/proveedores`, `licitaciones`, `ofertas`, `niveles-aprobacion` y `tipos-cambio`. |
| CRUD retorna códigos HTTP correctos (200, 201, 204, 400, 404, 409, 422 y 500 controlado). | Pruebas previas por módulo más `Error_BadRequest_…`, `Error_Conflicto_Duplicado_…`, `Error_NoEncontrado_…`, `Error_Negocio_PresupuestoSuperado_…` e `Error_Interno_NoControlado_…` en `ContratoApiRestHttpTests`. | Cada prueba fija el código esperado; `GetPorId_Inexistente_DebeResponder404` quedó retiquetada a `HU-26` con aserción reforzada sobre `ProblemDetails`. |
| Cualquier error usa ProblemDetails con detalle seguro y correlación, sin stack traces ni rutas internas. | Las mismas cinco pruebas de `ContratoApiRestHttpTests`. | Verifican `application/problem+json`, título, estado, detalle seguro y las extensiones `codigoError` y `correlacionId`; el caso interno además rechaza stack traces y rutas del proyecto. |
| Listados con paginación, filtrado y ordenamiento vía query params. | Pruebas previas de proveedores, licitaciones y ofertas. | Sin duplicación: HU-26 no agregó escenarios de listado nuevos. |

La ejecución focalizada con
`dotnet test tests\Licitaciones.IntegrationTests\Licitaciones.IntegrationTests.csproj --filter "HU=HU-26"`
terminó en ROJO con **5 fallidas y 0 correctas** (contrato ausente: sin
`codigoError`, sin `correlacionId` y 404 sin cuerpo) y tras el VERDE con **6
correctas, 0 fallidas y 0 omitidas**. La suite completa ejecutada con
`dotnet test Licitaciones.sln` pasó de **222** a **227 correctas, 0 fallidas y
0 omitidas**, y se mantuvo en 227 después del refactor.

La secuencia TDD queda trazada así:

- `9611c8d` — ROJO: agregó las cinco pruebas del contrato de errores; CI
  fallido como es esperable en rojo.
- `7db6e80` — VERDE: fábrica `FabricaProblemDetailsApi` registrada como
  `ProblemDetailsFactory`, constructor centralizado `RespuestaProblema` y
  manejador global que mapea `DomainException` a 422 y lo no previsto a un 500
  controlado; CI en verde.
- REFACTOR local sin commit: extrajo `ContratoProblemasApi` como única fuente
  de las claves y extensiones del contrato, delegó en ella la fábrica y
  `RespuestaProblema`, y reutilizó la fábrica dentro del manejador de
  excepciones de `Program.cs`, eliminando la construcción manual duplicada.

El PR [#66](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/66)
(`iteracion-3/hu-26-api-rest` hacia `main`) está abierto como draft. El
refactor permanece local, sin push ni CI registrado. La Issue #55 permanece
abierta y no se marca como completada desde esta fase.

## Resultado verificado para HU-27 (Iteración 3)

HU-27 corresponde a la Issue [#56](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/56).
Los dos criterios se cubren con una clase de integración HTTP sobre Swagger y
el documento OpenAPI, más una clase unitaria sobre `docs/api.md`:

| Criterio de aceptación de la Issue #56 | Pruebas | Evidencia |
| --- | --- | --- |
| `/swagger` muestra la documentación generada con todos los endpoints, esquemas de request/response y ejemplos. | `SwaggerUi_DebeServirInterfazEnRutaSwagger`, `DocumentoOpenApi_DebeExponerTodosLosEndpointsDelDominio`, `DocumentoOpenApi_DebeIncluirEsquemasRequestResponse` y `DocumentoOpenApi_DebeIncluirEjemplos` en `DocumentacionSwaggerHttpTests`. | El documento expone las 14 rutas del dominio; los esquemas incluyen `ProveedorDto`, `LicitacionDto`, `OfertaDto`, `TipoCambioDto`, `ProblemDetails` y `ValidationProblemDetails`; las operaciones POST/PUT declaran cuerpo `application/json` y cada esquema lleva ejemplo mediante `EjemplosEsquemasFiltro`. |
| `/docs/api.md` documenta endpoints, contratos, ejemplos y errores, y referencia una colección reproducible que existe. | `ApiMd_DebeDocumentarEndpointsContratosErroresYEjemplos` y `ApiMd_DebeReferenciarColeccionReproducibleExistenteYCubrirRecursos` en `DocumentacionApiMarkdownTests`. | La prueba valida recursos, errores con `ProblemDetails`/`codigoError`/`correlacionId` y bloques `json`/`http`; resuelve el archivo `.http` referenciado y verifica que cubra los cinco recursos del dominio (`docs/api.http`). |

La ejecución focalizada con
`dotnet test tests\Licitaciones.IntegrationTests\Licitaciones.IntegrationTests.csproj --filter "HU=HU-27"`
terminó en ROJO con **4 fallidas y 0 correctas** (Swagger UI y swagger.json
ausentes respondían 404) y tras el VERDE con **4 correctas**. La clase
unitaria pasó de 1 fallida (colección faltante) a 2 correctas. La suite
completa ejecutada con `dotnet test Licitaciones.sln` pasó de **227** a
**233 correctas, 0 fallidas y 0 omitidas**, y se mantuvo en 233 después del
refactor.

La secuencia TDD queda trazada así:

- `3af0427` — ROJO: agregó las seis pruebas de documentación interactiva; CI
  fallido como es esperable en rojo (ejecución `32682545858`).
- `b790880` — VERDE: Swashbuckle 7.2.0 con UI en Development, comentarios XML,
  ejemplos por esquema vía `EjemplosEsquemasFiltro`, sección HU-27 en
  `docs/api.md` y colección `docs/api.http`; CI en verde (ejecución
  `32684426351`).
- `14a8421` — REFACTOR local sin publicar: sustituyó la cadena ternaria del
  filtro por un diccionario estático tipo→ejemplo y renombró métodos ambiguos;
  sin comportamiento nuevo ni CI registrado.

Actualización al cierre de la Iteración 3: el PR
[#67](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/67)
(`iteracion-3/hu-27-swagger` hacia `main`) quedó fusionado como `666f175` con
CI `success`, después de publicarse también el refactor `14a8421`; la Issue
#56 se cerró un minuto después del merge.

## Resultado verificado para el cierre de la Iteración 3

Cierre documental del 24 de agosto de 2026 sobre el commit `666f175` de
`main`, sin modificar código. Las diez historias de la iteración (HU-18 a
HU-27) están fusionadas mediante los PR #58 a #67 y sus Issues (#47 a #56)
quedaron cerradas inmediatamente después de cada fusión; las ejecuciones de CI
de los diez commits de fusión terminaron en `success` (consultadas en la API
pública de GitHub).

La suite completa local sobre ese commit:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 85 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 132 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 16 | 0 | 0 |
| **Total ejecutado** | **233** | **0** | **0** |

`dotnet format Licitaciones.sln --verify-no-changes --no-restore` terminó sin
diferencias. La progresión de la suite durante la iteración fue: 175 (cierre
Iteración 2) → 187 (HU-18) → 191 (HU-19) → 193 (HU-20) → 199 (HU-21) → 204
(HU-22) → 216 (HU-23) → 219 (HU-24) → 222 (HU-25) → 227 (HU-26) → 233 (HU-27).

Velocidad registrada al cierre: planificada de referencia 36 SP, alcance
seleccionado 38 SP, velocidad observada 38 SP (las diez historias cumplen la
Definition of Done). Los detalles de fusión, trazabilidad de Issues,
ciclos TDD, participación y ajustes para la Iteración 4 constan en
`bitacora-xp.md`.

## Resultado verificado para HU-28 (Iteración 4)

HU-28 corresponde a la Issue [#69](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/69).
Sus dos criterios se cubren con pruebas unitarias de dominio y aplicación más
medición de cobertura con coverlet:

| Criterio de aceptación de la Issue #69 | Pruebas | Evidencia |
| --- | --- | --- |
| Cada regla de negocio (presupuesto/oferta > 0, oferta duplicada, oferta sobre presupuesto, estado no publicado, vencimiento, normalización de proveedor, código único, mejor oferta y desempate, clasificación de ahorro, nivel de aprobación, conversión CRC/USD, transiciones de estado) cuenta con al menos una prueba unitaria previa o concurrente. | Las reglas previas conservaban pruebas unitarias desde las Iteraciones 1 a 3; las áreas sin ninguna prueba directa (`TipoCambio`, `AdministrarTipoCambioService`, `NivelAprobacion`, `AdministrarNivelesAprobacionService`, `ConsultarOfertaService`) quedaron cubiertas con 34 casos nuevos en cinco clases: `TipoCambioTests`, `AdministrarTipoCambioServiceTests`, `NivelAprobacionTests`, `AdministrarNivelesAprobacionServiceTests` y `ConsultarOfertaServiceTests`. | La conversión CRC/USD se prueba dividiendo montos por el tipo activo (10 000 → 20 USD con tipo 500), exigiendo moneda CRC o USD, rechazando USD sin tipo activo y marcando la mejor oferta por monto y antigüedad. |
| La cobertura de líneas Domain/Application alcanza al menos 80 %. | Medición con `dotnet test --collect:"XPlat Code Coverage"` sobre `Licitaciones.UnitTests`. | Baseline sin HU-28: Application 52.93 %, Domain 70.90 % (estado rojo del criterio). Tras HU-28: Application **82.68 %**, Domain **89.29 %**. |

La secuencia TDD queda trazada así:

- `c0322b9` — ROJO: agregó los 34 casos unitarios con trait `HU-28`. Los casos
  pasaron individualmente porque el comportamiento ya estaba implementado (el
  criterio admite pruebas «previas o concurrentes»); el ROJO quedó registrado
  en la métrica de cobertura por debajo del umbral. Por eso CI terminó en
  `success` también en esta fase.
- `285972e` — VERDE: consolidó la infraestructura extrayendo
  `RepositorioTipoCambioEnMemoria` compartido y simplificando las clases de
  servicio; sin código de producción modificado en todo el ciclo.

La ejecución focalizada de las áreas nuevas terminó con **34 correctas, 0
fallidas y 0 omitidas**, y la suite completa con `dotnet test Licitaciones.sln`
pasó de **233** a **267 correctas, 0 fallidas y 0 omitidas** (119 unitarias,
132 de integración, 16 funcionales). El PR
[#80](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/80)
(`iteracion-4/hu-28-cobertura-pruebas` hacia `main`) está abierto y mergeable
con CI en verde en ambos commits; la Issue #69 permanece abierta y no se marca
como completada desde esta fase.

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
