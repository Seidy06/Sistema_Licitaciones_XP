# Bitácora XP

## Iteración 4 — Consolidación de pruebas, despliegue y entrega

**Estado: INICIADA.**

### Objetivo de la pequeña liberación

Consolidar las pruebas y la cobertura del dominio, contenerizar la aplicación
con Docker y Docker Compose, desplegarla mediante manifiestos de Kubernetes,
completar el pipeline de integración continua con análisis y auditoría de
dependencias, cerrar la documentación técnica en `/docs` y etiquetar la
entrega evaluable final.

### Planning Game y velocidad planificada

Se seleccionan HU-28 a HU-37 en el orden del catálogo. Suman **45 SP**. La
velocidad planificada de referencia permanece en **36 SP**; los 9 SP
adicionales se registran como riesgo explícito de alcance, porque el release
final concentra pruebas, contenedores, Kubernetes, CI completo, documentación
y etiquetado. No existe todavía velocidad observada de la Iteración 4 y no se
calculará hasta contar con evidencia de cierre. Ninguna historia está marcada
como terminada.

El orden y las dependencias son: HU-28 consolida las pruebas unitarias y su
cobertura; HU-29 formaliza la integración contra PostgreSQL real con la
infraestructura Testcontainers existente; HU-30 añade pruebas E2E de navegador
sobre la experiencia web completa de la Iteración 3; HU-31 introduce el
`Dockerfile` multi-stage; HU-32 orquesta el entorno local y depende de HU-31;
HU-33 y HU-34 trasladan aplicación y persistencia a Kubernetes en ese orden;
HU-35 integra todo en el pipeline de CI; HU-36 cierra la documentación técnica;
HU-37 etiqueta la entrega evaluable. El desarrollo aplicará ciclos TDD
rojo–verde–refactor, programación en pareja, integración continua y pequeñas
liberaciones; las Issues #69 a #78 se usan únicamente como tarjetas de
trazabilidad XP, no como backlog: sus criterios permanecen sin marcar hasta
que exista evidencia real en pruebas, commits, PR y CI.

### Pareja, trazabilidad y ramas previstas

La primera sesión queda asignada con **Tiffany como Driver/responsable
principal** y **Seidy como Navigator/revisión** para HU-28. La pareja rota los
roles en cada historia prevista:

| Orden | HU | Prioridad | SP | Driver | Navigator | Issue | Estado inicial | Rama prevista |
| ---: | --- | --- | ---: | --- | --- | --- | --- | --- |
| 1 | HU-28 — Pruebas unitarias del dominio | Alta | 5 | Tiffany | Seidy | [#69](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/69) | OPEN; no iniciada | `iteracion-4/hu-28-pruebas-unitarias-dominio` |
| 2 | HU-29 — Integración PostgreSQL real | Alta | 5 | Seidy | Tiffany | [#70](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/70) | OPEN; no iniciada | `iteracion-4/hu-29-integracion-postgresql` |
| 3 | HU-30 — Pruebas E2E de navegador | Alta | 8 | Tiffany | Seidy | [#71](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/71) | OPEN; no iniciada | `iteracion-4/hu-30-pruebas-e2e` |
| 4 | HU-31 — Dockerfile multi-stage | Alta | 3 | Seidy | Tiffany | [#72](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/72) | OPEN; no iniciada | `iteracion-4/hu-31-dockerfile` |
| 5 | HU-32 — Docker Compose local | Alta | 3 | Tiffany | Seidy | [#73](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/73) | OPEN; no iniciada | `iteracion-4/hu-32-docker-compose` |
| 6 | HU-33 — Manifiestos K8s de la app | Alta | 5 | Seidy | Tiffany | [#74](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/74) | OPEN; no iniciada | `iteracion-4/hu-33-k8s-app` |
| 7 | HU-34 — Persistencia PostgreSQL en K8s | Alta | 5 | Tiffany | Seidy | [#75](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/75) | OPEN; no iniciada | `iteracion-4/hu-34-k8s-postgresql` |
| 8 | HU-35 — Pipeline de CI completo | Alta | 5 | Seidy | Tiffany | [#76](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/76) | OPEN; no iniciada | `iteracion-4/hu-35-pipeline-ci` |
| 9 | HU-36 — Documentación final en /docs | Alta | 5 | Tiffany | Seidy | [#77](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/77) | OPEN; no iniciada | `iteracion-4/hu-36-documentacion-final` |
| 10 | HU-37 — Etiquetado de entrega final | Alta | 1 | Seidy | Tiffany | [#78](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/78) | OPEN; no iniciada | `iteracion-4/hu-37-tag-entrega` |

Git conservará la autoría del Driver de cada incremento; el rol Navigator se
reconstruirá a partir del trabajo coordinado de la pareja, sin atribuir
sesiones sin evidencia.

### Ajustes heredados de la Iteración 3

Los ajustes registrados en el cierre de la Iteración 3 alimentan esta
iteración: mensajería invisible en dos flujos, variante de advertencia sin
productores, ícono de tema estático, formato ₡ faltante en
`NivelesAprobacion/Delete`, residuos `WeatherForecast`, duplicaciones de
fábricas de pruebas y correcciones documentales de referencias. Se resolverán
dentro de las historias de esta iteración cuando correspondan a su alcance o
quedarán registradas en la documentación final.

### HU-28 — Configurar TDD y pipeline de pruebas unitarias del dominio

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-28 — Configurar TDD y pipeline de pruebas unitarias del dominio | 5 | [#69](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/69) | Criterios cubiertos por pruebas en verde con cobertura medida; la Issue permanece abierta y no se marca como completada ni se cierra desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-28 | Seidy | Tiffany | `c0322b9` |
| VERDE HU-28 | Tiffany | Seidy | `285972e` |

La asignación planificada para la primera sesión era Tiffany Driver/Seidy
Navigator; la autoría real de los commits muestra el ROJO firmado por Seidy y
el VERDE por Tiffany. Git conserva la autoría del Driver de cada incremento;
el rol Navigator se reconstruye a partir del trabajo coordinado de la pareja,
sin atribuir sesiones sin evidencia.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

La Issue #69 se contrastó con `docs/historias-usuario.md` antes de programar:
título, prioridad Alta, estimación 5 SP, iteración 4 (RELEASE 8) y los dos
criterios coinciden literalmente.

Observación de trazabilidad: la rama real
(`iteracion-4/hu-28-cobertura-pruebas`) difiere de la prevista en el Planning
Game (`iteracion-4/hu-28-pruebas-unitarias-dominio`); se registra sin corregir
en fase. Los commits usan `refs #69` correctamente.

Inspección previa para no duplicar escenarios: presupuesto/oferta mayores que
cero, oferta duplicada, oferta sobre presupuesto, estado no publicado,
vencimiento con `FixedClock`, normalización y código único de proveedor,
código único de licitación, mejor oferta con desempate (HU-16), clasificación
de ahorro y transiciones Publicar/Cerrar ya contaban con pruebas unitarias.
Quedaban sin ninguna prueba unitaria directa: `TipoCambio`,
`AdministrarTipoCambioService`, `NivelAprobacion`,
`AdministrarNivelesAprobacionService` y `ConsultarOfertaService`
(conversión CRC/USD, filtro, orden y paginación).

| Criterio de aceptación de la Issue #69 | Pruebas | Commits |
| --- | --- | --- |
| Cada regla de negocio listada cuenta con al menos una prueba unitaria previa o concurrente que la cubre. | Las reglas previas conservaron sus pruebas; las áreas huérfanas quedaron cubiertas con las clases `TipoCambioTests`, `AdministrarTipoCambioServiceTests`, `NivelAprobacionTests`, `AdministrarNivelesAprobacionServiceTests` y `ConsultarOfertaServiceTests` (34 casos nuevos con trait `HU-28`). | ROJO `c0322b9`; VERDE `285972e`. |
| La cobertura de líneas Domain/Application alcanza al menos 80 %. | Medición con coverlet (`--collect:"XPlat Code Coverage"`): baseline sin las pruebas nuevas Application 52.93 % y Domain 70.90 %; tras el incremento Application 82.68 % y Domain 89.29 %. | Ídem. |

El PR [#80](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/80)
(`iteracion-4/hu-28-cobertura-pruebas` hacia `main`) está abierto y mergeable,
con ambos commits publicados y CI en verde (check `Build and Test` en
`success` para `c0322b9` y `285972e`).

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `c0322b9` — `test(calidad): cubrir criterios de configurar tdd y pipeline de pruebas unitarias del dominio (HU-28)` | Agregó cinco clases de prueba con 34 casos unitarios (trait `HU-28`) sobre las áreas sin cobertura: validación y monedas predeterminadas de `TipoCambio`; guardado/reemplazo activo, consulta nula, orden/paginación y validaciones de `AdministrarTipoCambioService`; validaciones de rango, normalización de nombre y desactivación de `NivelAprobacion`; traslape/conflicto, creación, desactivación, filtro y orden de `AdministrarNivelesAprobacionService`; conversión CRC/USD, mejor oferta por monto y antigüedad, moneda no soportada, USD sin tipo activo, filtro proveedor y paginación de `ConsultarOfertaService`. Los 34 casos pasaron individualmente porque el comportamiento ya estaba implementado —el criterio admite pruebas «previas o concurrentes»—; el ROJO real del ciclo quedó registrado en la métrica del segundo criterio: cobertura Application 52.93 % y Domain 70.90 %, por debajo del umbral de 80 %. Por eso CI terminó en `success` también en esta fase. |
| VERDE | `285972e` — `feat(calidad): implementar configurar tdd y pipeline de pruebas unitarias del dominio (HU-28)` | Consolidó la infraestructura de pruebas extrayendo el repositorio falso duplicado de tipo de cambio a `RepositorioTipoCambioEnMemoria` compartido en `Common`, simplificando las dos clases de servicio; sin código de producción modificado en todo el ciclo. Filtro focalizado 34/34 correctas; suite completa en verde; CI en `success`. |
| REFACTOR | Sin commit dedicado | No se identificó refactorización adicional justificada dentro del alcance de la historia; la consolidación de la infraestructura quedó incluida en el VERDE. |

#### Resultado de pruebas (HU-28)

La línea base previa al incremento estaba verde con 233 pruebas en la
solución (85 unitarias). El ROJO no produjo fallos de prueba porque las reglas
ya existían; el umbral de cobertura del criterio 2 era el estado rojo medido
(52.93 % / 70.90 %). Tras el incremento:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 119 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 132 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 16 | 0 | 0 |
| **Total ejecutado** | **267** | **0** | **0** |

Cobertura de líneas con coverlet sobre el HEAD:

| Capa | Baseline sin HU-28 | Tras HU-28 | Umbral |
| --- | ---: | ---: | ---: |
| `Licitaciones.Domain` | 70.90 % | **89.29 %** | ≥ 80 % |
| `Licitaciones.Application` | 52.93 % | **82.68 %** | ≥ 80 % |

`dotnet format Licitaciones.sln --verify-no-changes --no-restore` terminó sin
diferencias.

#### Pendientes y candidatos a Issues separadas

- La discrepancia entre la rama prevista
  (`iteracion-4/hu-28-pruebas-unitarias-dominio`) y la real
  (`iteracion-4/hu-28-cobertura-pruebas`) se reporta sin ocultar; no afecta
  trazabilidad de Issue ni commits.
- El pipeline de CI todavía no mide cobertura ni aplica el umbral de 80 %
  automáticamente; integrarlo corresponde a HU-35 (#76).
- La rotación Driver/Navigator observada invierte la pareja planificada para
  esta historia (ROJO Seidy/Tiffany, VERDE Tiffany/Seidy); se reconstruye por
  autoría de commits.

La Issue #69 permanece abierta.

## Iteración 3 — Aprobación, conversión, experiencia web y API documentada

**Estado: INICIADA.**

### Objetivo de la pequeña liberación

Incorporar niveles de aprobación parametrizables, administración y conversión
CRC/USD, una experiencia web completa con navegación, tema, mensajería y
formato `es-CR`, y una API REST versionada documentada mediante OpenAPI/Swagger.

### Planning Game y velocidad planificada

Se seleccionan HU-18 a HU-27 en el orden del catálogo. Suman **38 SP**. La
velocidad planificada de referencia permanece en **36 SP**; los 2 SP adicionales
se registran como riesgo explícito de alcance. No existe todavía velocidad
observada de la Iteración 3 y no se calculará hasta contar con evidencia de
cierre. Ninguna historia está marcada como terminada.

El orden y las dependencias son: HU-18 y HU-19 establecen las capacidades de
negocio; HU-20 inicia la experiencia informativa; HU-21 establece la navegación;
HU-22 depende del layout; HU-23 integra los casos de uso en MVC; HU-24 y HU-25
uniforman retroalimentación y presentación; HU-26 consolida los contratos REST;
HU-27 documenta interactivamente esos contratos. El desarrollo aplicará ciclos
TDD rojo–verde–refactor, programación en pareja, integración continua y
pequeñas liberaciones; las Issues se usan únicamente como trazabilidad XP.

### Pareja, trazabilidad y ramas previstas

La primera sesión queda asignada con **Tiffany como Driver/responsable
principal** y **Seidy como Navigator/revisión** para HU-18. La pareja rota los
roles en cada historia prevista:

| Orden | HU | Prioridad | SP | Driver | Navigator | Issue | Estado inicial | Rama prevista |
| ---: | --- | --- | ---: | --- | --- | --- | --- | --- |
| 1 | HU-18 | Alta | 5 | Tiffany | Seidy | [#47](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/47) | OPEN; no iniciada | `iteracion-3/hu-18-niveles-aprobacion` |
| 2 | HU-19 | Alta | 5 | Seidy | Tiffany | [#48](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/48) | OPEN; no iniciada | `iteracion-3/hu-19-tipo-cambio` |
| 3 | HU-20 | Media | 3 | Tiffany | Seidy | [#49](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/49) | OPEN; no iniciada | `iteracion-3/hu-20-landing-page` |
| 4 | HU-21 | Media | 2 | Seidy | Tiffany | [#50](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/50) | OPEN; no iniciada | `iteracion-3/hu-21-navegacion-global` |
| 5 | HU-22 | Baja | 2 | Tiffany | Seidy | [#51](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/51) | OPEN; no iniciada | `iteracion-3/hu-22-tema-claro-oscuro` |
| 6 | HU-23 | Alta | 8 | Seidy | Tiffany | [#52](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/52) | OPEN; criterios verificados localmente; PR abierto | `iteracion-3/hu-23-crud-web` |
| 7 | HU-24 | Media | 2 | Tiffany | Seidy | [#53](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/53) | OPEN; ROJO y VERDE en rama con CI verde; refactor local sin commit | `iteracion-3/hu-24-mensajeria` |
| 8 | HU-25 | Baja | 1 | Seidy | Tiffany | [#54](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/54) | OPEN; ROJO y VERDE publicados con CI (rojo esperable, verde en success); refactor en commit local sin publicar | `iteracion-3/hu-25-formato-es-cr` |
| 9 | HU-26 | Alta | 8 | Tiffany | Seidy | [#55](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/55) | OPEN; ROJO y VERDE publicados con CI (rojo esperable, verde en success); refactor local sin commit. Rama real difiere de la prevista | `iteracion-3/hu-26-api-rest` |
| 10 | HU-27 | Media | 2 | Seidy | Tiffany | [#56](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/56) | OPEN; ROJO y VERDE publicados con CI (rojo esperable, verde en success); REFACTOR en commit local sin publicar | `iteracion-3/hu-27-swagger` |

Las Issues son exclusivamente tarjetas de trazabilidad XP. Sus criterios
permanecen sin marcar hasta que exista evidencia real en pruebas, código,
integración continua y documentación. No se registran commits, Pull Requests,
resultados de CI ni pequeñas liberaciones que aún no existen.

### HU-18 — Administrar niveles de aprobación

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-18 — Administrar niveles de aprobación | 5 | Criterios de aceptación cubiertos y verificados: creación con rechazo de traslapes en servidor y base de datos, rechazo del segundo rango abierto y resolución del aprobador consultando la tabla. La creación y la resolución están expuestas por API; las operaciones de editar, listar y desactivar mencionadas en el enunciado permanecen fuera de este incremento y se reportan como alcance restante. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-18 | Tiffany | Seidy | `bd1f3d6` |
| VERDE HU-18 | Seidy | Tiffany | `249ab70` |
| Refactor HU-18 | Tiffany | Seidy | `1224ece` |

Los roles conservan la asignación planificada (Tiffany Driver, Seidy Navigator)
y se reconstruyen a partir de la autoría alternada de los commits; Git conserva
la autoría del Driver, no evidencia independiente del rol Navigator.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `bd1f3d6` — `test(aprobacion): cubrir criterios de administrar niveles de aprobación (tabla parametrizable) (HU-18)` | Agregó cinco pruebas de integración sobre PostgreSQL real: dos de persistencia para la restricción de exclusión (traslape y segundo rango abierto, error `23P01`) y tres HTTP (`409` sin persistir el segundo nivel, segundo rango abierto rechazado y resolución desde la tabla con restauración del catálogo). CI falló como es esperable en rojo (ejecución `32532418505`). |
| VERDE | `249ab70` — `feat(aprobacion): implementar administrar niveles de aprobación (tabla parametrizable) (HU-18)` | Incorporó `NivelAprobacion.Crear`, `AdministrarNivelesAprobacionService`, `ResolverNivelAprobacionService`, `INivelAprobacionRepository`, `NivelAprobacionRepository`, el controlador `NivelesAprobacionController` con registro DI, la migración `AdministrarNivelesAprobacionHu18` (columna `Activo`, restricción de exclusión filtrada y secuencia de identificadores) y la consulta activa del resolutor. CI en `success` (ejecución `32534822110`). |
| Refactor | `1224ece` — `refactor(aprobacion): simplificar implementacion de HU-18` | Renombró `ResolverNivelAprobacion` a `ResolverAsync` (redundancia con el nombre de la clase), alineó el helper `Problema` con la convención `CrearProblema` del resto de controladores y eliminó una línea sobrante en la interfaz del repositorio. Sin comportamiento nuevo; build sin errores ni advertencias. CI en `success` (ejecución `32556366636`). |

#### Commits

- `bd1f3d6` — pruebas de criterios de aceptación (ROJO).
- `249ab70` — implementación mínima (VERDE).
- `1224ece` — refactorización sin cambio funcional.

#### Resultado de pruebas (HU-18)

La suite completa se ejecutó localmente antes y después del refactor el 22 de
agosto de 2026 con `dotnet test Licitaciones.sln` y PostgreSQL real mediante
Testcontainers. Resultado final:

- 83 pruebas unitarias superadas.
- 101 pruebas de integración superadas (incluye las 5 de HU-18).
- 3 pruebas funcionales superadas.
- 0 fallidas y 0 omitidas; 187 ejecutadas.

#### Pendientes y candidatos a Issues separadas

1. Las operaciones de editar, listar y desactivar del enunciado de HU-18 no
   están implementadas; solo creación y resolución quedaron cubiertas por los
   criterios de aceptación de la Issue #47.
2. Duplicación del helper `CrearProblema` en los cuatro controladores API,
   con inconsistencia adicional en el campo `Type` de `ProblemDetails`.
3. Ausencia de pruebas unitarias de `AdministrarNivelesAprobacionService` y
   `ResolverNivelAprobacionService`; su cobertura actual es únicamente de
   integración.
4. `ResolverNivelAprobacionService` depende de `ILicitacionConsultaRepository`
   del módulo de consulta de licitaciones en lugar de una abstracción propia
   del módulo de aprobaciones.

Estos puntos no se ocultaron dentro de HU-18: se registran como candidatos a
Issues separadas. La Issue #47 permanece abierta y no se cierra desde esta fase.

### HU-19 — Administrar tipo de cambio y conversión CRC/USD

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-19 — Administrar tipo de cambio y conversión CRC/USD | 5 | Criterios de aceptación cubiertos y verificados: guardar un nuevo tipo de cambio desactiva automáticamente el previo (un único activo, respaldado además por el índice único parcial `UX_TiposCambio_Activo`), la conversión USD es solo de presentación (`monto / tipoCambio.Valor`) sin modificar el valor persistido en colones, la respuesta en USD incluye el valor y la fecha del tipo de cambio utilizado, y la conversión funciona sin conexión externa consultando el registro administrado localmente. La alternancia de moneda desde la interfaz web aún no existe; la exposición actual es por API y las vistas pertenecen a las historias web de la iteración. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-19 | Seidy | Tiffany | `78be404` |
| VERDE HU-19 | Tiffany | Seidy | `7ab8e09` |
| Refactor HU-19 | Seidy | Tiffany | `ff92f44` |

Los roles conservan la asignación planificada (Seidy Driver, Tiffany
Navigator) con rotación por fase y se reconstruyen a partir de la autoría
alternada de los commits; Git conserva la autoría del Driver, no evidencia
independiente del rol Navigator.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `78be404` — `test(moneda): cubrir criterios de administrar tipo de cambio y conversión crc/usd (HU-19)` | Agregó cuatro pruebas HTTP sobre PostgreSQL real con trait `HU-19`: reemplazo del activo quedando un solo registro (con verificación directa en la tabla), conversión USD como presentación conservando el monto persistido en CRC, inclusión del valor y la fecha del tipo de cambio utilizado, y conversión funcional bloqueando toda llamada saliente para simular ausencia de Internet. CI falló como es esperable en rojo (ejecución `32579740531`). |
| VERDE | `7ab8e09` — `feat(moneda): implementar administrar tipo de cambio y conversión crc/usd (HU-19)` | Incorporó `TipoCambio.Crear`/`Desactivar`, `AdministrarTipoCambioService`, `ITipoCambioRepository`, `TipoCambioRepository.ReemplazarActivoAsync`, `TiposCambioController` (`POST /api/v1/tipos-cambio` y `GET /api/v1/tipos-cambio/activo`) con registro DI, y amplió la consulta de ofertas con `tipoCambioValor`/`tipoCambioFecha` al solicitar USD. Sin migraciones nuevas: reutiliza la tabla, la semilla y el índice existentes desde la iteración 1. CI en `success` (ejecución `32597593088`). |
| Refactor | `ff92f44` — `refactor(moneda): simplificar implementacion de HU-19` | Centralizó el par de monedas administrado en las constantes `MonedaOrigenPredeterminada`/`MonedaDestinoPredeterminada` del dominio, eliminando los literales repetidos en repositorios, servicio y configuración; retiró la consulta duplicada `ObtenerTipoCambioUsdCrcAsync` de `IOfertaConsultaRepository`/`OfertaRepository` dejando `ITipoCambioRepository.ObtenerActivoAsync` como única fuente del registro activo; simplificó `ConvertirAsync` extrayendo `esDolares`. Sin comportamiento nuevo. Verificación local: build Release sin errores ni advertencias, `dotnet format --verify-no-changes` sin diferencias y suite completa en verde antes y después. El commit permanece local a la espera de publicarse en el PR #59, por lo que todavía no tiene ejecución de CI registrada. |

#### Commits

- `78be404` — pruebas de criterios de aceptación (ROJO).
- `7ab8e09` — implementación mínima (VERDE).
- `ff92f44` — refactorización sin cambio funcional (local, sin publicar).

#### Pull Request

El PR [#59](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/59)
(`iteracion-3/hu-19-tipo-cambio` hacia `main`) está abierto con los commits
ROJO y VERDE publicados; su verificación (`Build and Test`) está en verde sobre
el último commit publicado `7ab8e09`. El commit de refactor `ff92f44`
se incorporará al publicar la rama.

#### Resultado de pruebas (HU-19)

La suite completa se ejecutó localmente antes y después del refactor el 22 de
agosto de 2026 con `dotnet test Licitaciones.sln --configuration Release
--no-build` y PostgreSQL real mediante Testcontainers. Resultado final:

- 83 pruebas unitarias superadas.
- 105 pruebas de integración superadas (incluye las 4 de HU-19).
- 3 pruebas funcionales superadas.
- 0 fallidas y 0 omitidas; 191 ejecutadas.

#### Pendientes y candidatos a Issues separadas

1. La alternancia CRC/USD desde la interfaz web no está implementada: los
   criterios de HU-19 quedaron cubiertos por API (`?moneda=USD` retorna monto
   convertido, `tipoCambioValor` y `tipoCambioFecha`); las vistas MVC
   pertenecen a las historias web de la Iteración 3.
2. Mojibake preexistente en los mensajes de error de
   `ConsultarOfertaService.ValidarConsulta` («licitaciÃ³n», «paginaciÃ³n»,
   «ordenamiento»), introducido por el commit `fc87fe0` de HU-17 y detectado
   durante el refactor de HU-19; quedó fuera del alcance de la Issue #48.
3. Duplicación del helper `CrearProblema`: con `TiposCambioController` son ya
   cinco copias en los controladores API (pendiente registrado por HU-18 con
   cuatro).

Estos puntos no se ocultaron dentro de HU-19: se registran como candidatos a
Issues separadas. La Issue #48 permanece abierta y no se cierra ni marca sus
criterios desde esta fase.

### HU-20 — Landing page informativa

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-20 — Landing page informativa | 3 | Criterios de aceptación cubiertos y verificados: la ruta raíz `/` responde `200 OK` a un visitante sin autenticación mostrando las seis secciones explicativas (propósito de la aplicación, flujo de licitación, ofertas, mejor oferta, nivel de aprobación y conversión monetaria), y con un agente móvil se comprueba la meta viewport, la hoja de estilos Bootstrap, el cuerpo dentro de `<main>` y la rejilla por puntos de ruptura. La vista es Razor estática servida por un controlador delgado; el menú de navegación global pertenece a HU-21 y el CRUD web a HU-23. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO+VERDE HU-20 | Tiffany | Seidy | `8062619` |
| Refactor HU-20 | Seidy | Tiffany | Sin commit: evaluado y rechazado |

Los roles conservan la asignación planificada (Tiffany Driver, Seidy
Navigator) con rotación por fase y se reconstruyen a partir de la autoría
alternada de los commits; Git conserva la autoría del Driver, no evidencia
independiente del rol Navigator.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO+VERDE | `8062619` — `test(web): cubrir criterios de landing page informativa (HU-20)` | Publicó en un único commit las dos pruebas funcionales con trait `HU-20` (acceso anónimo a la raíz con las seis secciones explicativas y responsividad con agente móvil: viewport con `width=device-width`, Bootstrap, cuerpo en `<main>` y al menos tres clases de columna por breakpoint) junto con la implementación mínima: la vista Razor estática `Views/Home/Index.cshtml` con el encabezado y las tarjetas explicativas. Ajustó a `HU-00` el trait de la prueba de plantilla preexistente sin alterar sus aserciones. Desviación de proceso registrada: al combinarse pruebas e implementación en un solo commit no existe evidencia separada de ROJO ni ejecución de CI fallida para esta historia. CI en `success` (ejecución `32613192010`). |
| Refactor | Sin commit | Se evaluó eliminar la duplicación de los cinco bloques de tarjeta extrayéndolos a un bucle Razor; la extracción rompió la prueba porque `HtmlEncoder.Default` escapa los caracteres fuera de Basic Latin («licitaci&#243;n») y las alternativas (`Html.Raw` o reconfigurar el encoder global en `Program.cs`) eran compensaciones peores que la duplicación idiomática de una página estática. Se revirtió sin dejar cambios y la suite completa permaneció verde antes y después (193 pruebas). No se fabricó commit de refactor. |

#### Commits

- `8062619` — pruebas de los dos criterios de aceptación e implementación mínima de la vista (ROJO+VERDE combinados en un único commit publicado).

#### Pull Request

El PR [#60](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/60)
(`iteracion-3/hu-20-landing-page` hacia `main`) está abierto, en estado
mergeable, con el único commit `8062619`; su verificación (`Build and Test`)
está en verde (ejecución `32613192010`). La Issue #49 permanece abierta hasta
completar la Definition of Done.

#### Resultado de pruebas (HU-20)

La suite completa se ejecutó localmente el 22 de agosto de 2026 con
`dotnet test Licitaciones.sln` y PostgreSQL real mediante Testcontainers.
Resultado final:

- 83 pruebas unitarias superadas.
- 105 pruebas de integración superadas.
- 5 pruebas funcionales superadas (incluye las 2 de HU-20).
- 0 fallidas y 0 omitidas; 193 ejecutadas.

#### Pendientes y candidatos a Issues separadas

1. El setup de unas trece líneas de `WebApplicationFactory` se repite idéntico
   en las tres clases de pruebas funcionales (`PlantillaWebTests`,
   `CrearLicitacionFormTests` y `LandingPageWebTests`); extraerlo a un helper
   compartido toca pruebas de otras historias (HU-00 y HU-10) y quedó fuera
   del alcance de la Issue #49.
2. `_Layout.cshtml` declara `lang="en"` con contenido en español; afecta
   accesibilidad y SEO de todas las páginas y excede el alcance de esta HU.
3. Ciclo TDD publicado como un único commit que combina pruebas e
   implementación, sin evidencia separada de ROJO ni ejecución de CI fallida;
   se registra como desviación de proceso de esta historia.

Estos puntos no se ocultaron dentro de HU-20: se registran como candidatos a
Issues separadas. La Issue #49 permanece abierta y no se cierra ni marca sus
criterios desde esta fase.

### HU-21 — Menú de navegación global

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-21 — Menú de navegación global | 2 | [#50](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/50) | Criterios cubiertos y verificados localmente; la Issue permanece abierta y no se marca como completada desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-21 | Seidy | Tiffany | `a9dd711` |
| VERDE HU-21 | Tiffany | Seidy | `0e226e7` |
| Ajuste de estilo HU-21 | Tiffany | Seidy | `b206f9c` |
| Refactor HU-21 | Seidy | Tiffany | `e2fbd06` |

Los roles siguen la asignación planificada para HU-21 y se contrastan con la
autoría alternada de los commits; Git conserva la autoría del Driver, no una
evidencia independiente del rol Navigator.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

| Criterio de aceptación de la Issue #50 | Prueba | Evidencia de commits |
| --- | --- | --- |
| Cualquier página del sitio muestra el menú global y resalta la sección activa. | `Layout_CualquierPagina_DebeMostrarMenuGlobalConTodosLosModulos`, `Layout_EnPaginaInicio_DebeResaltarSeccionActiva` y `Layout_EnPaginaDeOtraSeccion_DebeMoverElResaltadoALaSeccionCorrespondiente` en `NavegacionGlobalWebTests`. | ROJO `a9dd711`; VERDE `0e226e7`; ajuste de estilo `b206f9c`; REFACTOR `e2fbd06`. |
| El enlace de documentación de API abre Swagger UI. | `EnlaceADocumentacionApi_DebeAbrirSwaggerUi` en `NavegacionGlobalWebTests`. | ROJO `a9dd711`; VERDE `0e226e7`; ajuste de estilo `b206f9c`; REFACTOR `e2fbd06`. |

El ROJO agregó las pruebas funcionales con trait `HU-21`; el VERDE incorporó
el layout y el partial de navegación; el ajuste de estilo ordenó imports sin
cambiar comportamiento; y el REFACTOR redujo la duplicación del marcado de
las secciones MVC con una colección de rutas y etiquetas, sin ampliar la
Issue. La prueba filtrada de HU-21 terminó con 6 correctas, 0 fallidas y 0
omitidas. La suite completa local del 22 de agosto de 2026 terminó con 199
correctas, 0 fallidas y 0 omitidas.

#### Pull Request

No existe un Pull Request de HU-21 creado o verificable en esta fase; por tanto
no se inventa un número ni un estado de CI. La rama actual contiene los cuatro
commits anteriores y la Issue #50 permanece abierta, sin cerrarse ni marcarse
como completada.

#### Pendientes y candidatos a Issues separadas

No se identificó trabajo nuevo durante esta fase de refactor. Los cambios se
limitaron al partial de navegación y no modificaron API, modelo de datos,
Docker ni interfaces CRUD de otros módulos.

### HU-22 — Modo claro/oscuro persistente

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-22 — Modo claro/oscuro persistente | 2 | [#51](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/51) | Criterios cubiertos y verificados localmente (ROJO confirmado, VERDE y suite completa en verde, REFACTOR aplicado); la Issue permanece abierta y no se marca como completada ni se cierra desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-22 | Tiffany | Seidy | `12aa5c4` |
| VERDE HU-22 | Seidy | Tiffany | `9975a05` |
| Refactor HU-22 | Sin commit | Sin commit | Cambios locales pendientes de publicar |

Los roles del ROJO y del VERDE se reconstruyen a partir de la autoría
alternada de los commits (`12aa5c4` firmado por Tiffany, `9975a05` por Seidy);
Git conserva la autoría del Driver, no una evidencia independiente del rol
Navigator. El refactor aún no tiene commit, por lo que sus roles quedan sin
registrar hasta que exista esa evidencia.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

| Criterio de aceptación de la Issue #51 | Prueba | Evidencia de commits |
| --- | --- | --- |
| Given el control de tema, When se cambia, Then la preferencia persiste entre sesiones (almacenamiento local del navegador). | `Layout_CualquierPagina_DebeMostrarControlVisibleParaAlternarTema` y `ControlDeTema_AlCambiar_DebePersistirPreferenciaEntreSesionesEnLocalStorage` en `TemaClaroOscuroWebTests`. | ROJO `12aa5c4`; VERDE `9975a05`; REFACTOR local sin commit. |
| Given una nueva visita, When se carga la página, Then se respeta el último tema seleccionado. | `NuevaVisita_AlCargarPagina_DebeRespetarUltimoTemaSeleccionado` en `TemaClaroOscuroWebTests`. | ROJO `12aa5c4`; VERDE `9975a05`; REFACTOR local sin commit. |

El ROJO agregó cinco casos funcionales HTTP con trait `HU-22` en
`TemaClaroOscuroWebTests` y su ejecución filtrada terminó con 5 fallidas y 0
superadas por comportamiento ausente: sin control `theme-toggle`, sin lógica de
`localStorage` y sin paleta oscura. El VERDE incorporó el botón accesible en el
partial de navegación, el script inicial contra parpadeo en el layout, la
alternancia con persistencia en `site.js` y la paleta oscura en `site.css`;
la ejecución filtrada quedó 5 correctas, 0 fallidas y 0 omitidas. El REFACTOR,
aplicado localmente el 23 de agosto de 2026 sin commit todavía, eliminó la
aplicación inicial del tema duplicada en `site.js` (quedó como única
responsabilidad del script del layout), inlineó el helper usado una sola vez y
añadió el salto de línea final de `site.css`, sin comportamiento nuevo. La
suite completa posterior al refactor terminó con 204 correctas, 0 fallidas y 0
omitidas.

#### Commits

- `12aa5c4` — `test(web): cubrir criterios de modo claro/oscuro persistente (HU-22)` (ROJO).
- `9975a05` — `feat(web): implementar modo claro/oscuro persistente (HU-22)` (VERDE).
- REFACTOR: cambios locales en `site.js` y `site.css` sin commit ni push en esta fase.

#### Pull Request

El PR [#62](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/62)
(`iteracion-3/hu-22-modo-claro-oscuro` hacia `main`) está abierto, en estado
mergeable, con los commits `12aa5c4` y `9975a05`; su verificación
(`Build and Test`) está en verde. Los cambios del refactor permanecen locales y
aún no forman parte del PR. La Issue #51 permanece abierta, sin cerrarse ni
marcarse como completada.

Nota de trazabilidad: la rama prevista registrada en Planning Game era
`iteracion-3/hu-22-tema-claro-oscuro`, pero la rama real de trabajo es
`iteracion-3/hu-22-modo-claro-oscuro`; la diferencia de nombre no altera
alcance, criterios ni contenido de la historia.

#### Resultado de pruebas (HU-22)

Ejecuciones locales del 23 de agosto de 2026 con PostgreSQL real mediante
Testcontainers:

1. Fase ROJO, filtrada con `dotnet test tests\Licitaciones.FunctionalTests --filter "HU=HU-22"`:
   5 fallidas y 0 superadas, todas por aserciones de comportamiento ausente.
2. Fase VERDE, misma ejecución filtrada: 5 correctas, 0 fallidas, 0 omitidas.
3. Tras el refactor, suite completa con `dotnet test Licitaciones.sln`:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 83 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 105 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 16 | 0 | 0 |
| **Total ejecutado** | **204** | **0** | **0** |

#### Pendientes y candidatos a Issues separadas

1. El ícono del control de tema (&#9788;) es fijo y no refleja el tema activo;
   mostrar un ícono distinto por tema es comportamiento nuevo fuera del alcance
   de la Issue #51.
2. La colección de páginas `PaginasDelSitio` se duplica entre
   `NavegacionGlobalWebTests` (HU-21) y `TemaClaroOscuroWebTests` (HU-22);
   extraerla a un dato compartido toca pruebas de otra historia.
3. Discrepancia entre el nombre de rama previsto y el real, registrada como
   nota de trazabilidad.

Estos puntos no se ocultaron dentro de HU-22: se registran como candidatos a
Issues separadas. La Issue #51 permanece abierta y no se cierra ni marca sus
criterios desde esta fase.

### HU-23 — CRUD completo desde la interfaz web

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-23 — CRUD completo desde la interfaz web para todos los módulos | 8 | [#52](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/52) | Criterios cubiertos y verificados localmente; la Issue permanece abierta y no se marca como completada ni se cierra desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-23 | Seidy | Tiffany | `b5ff1fe` |
| VERDE HU-23 | Tiffany | Seidy | `e4b7973` |
| Corrección de imports | Seidy | Tiffany | `5b3be34` |
| REFACTOR HU-23 | Seidy | Tiffany | `2803c00` |

Los roles se registran según la asignación de la pareja para cada incremento;
Git conserva la autoría de los commits, pero no evidencia independiente del rol
Navigator.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

| Criterio de aceptación de la Issue #52 | Pruebas | Commits |
| --- | --- | --- |
| Listados con paginación, filtrado y ordenamiento. | Las cinco pruebas de `CrudWebListadosTests`, una por módulo MVC. | ROJO `b5ff1fe`; VERDE `e4b7973`; corrección `5b3be34`; REFACTOR `2803c00`. |
| Validación junto al campo y conservación de datos inválidos. | Las cinco pruebas de `CrudWebFormulariosInvalidosTests`, una por módulo MVC. | ROJO `b5ff1fe`; VERDE `e4b7973`; corrección `5b3be34`; REFACTOR `2803c00`. |
| Confirmación antes de cualquier eliminación permitida. | Las dos pruebas de `CrudWebConfirmacionEliminacionTests` para proveedores y niveles de aprobación. | ROJO `b5ff1fe`; VERDE `e4b7973`; corrección `5b3be34`; REFACTOR `2803c00`. |

El PR [#63](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/63)
(`iteracion-3/hu-23-crud-web` hacia `main`) está abierto y mergeable. Los
commits `b5ff1fe`, `e4b7973` y `5b3be34` están publicados en la rama remota;
`2803c00` es local y todavía no forma parte del PR. No se atribuye un resultado
de CI al refactor local.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `b5ff1fe` — `test(web): cubrir criterios de crud completo desde la interfaz web para todos los módulos (HU-23)` | Agregó 12 pruebas de integración HTTP con trait `HU-23`: cinco listados, cinco formularios inválidos y dos confirmaciones de eliminación. La ejecución inicial documentó el comportamiento ausente esperado en TDD. |
| VERDE | `e4b7973` — `feat(web): implementar crud completo desde la interfaz web para todos los módulos (HU-23)` | Incorporó las acciones MVC, ViewModels, vistas, servicios y registro DI necesarios para los cinco módulos, respetando DTOs/ViewModels y las eliminaciones permitidas. |
| Corrección | `5b3be34` — `fix(web): corregir orden de importaciones para CI (HU-23) refs #52` | Ajustó el orden de imports sin cambiar comportamiento ni criterios. |
| REFACTOR | `2803c00` — `refactor(web): simplificar implementacion de HU-23` | Movió `PaginaResultado<T>` desde el namespace de Proveedores a `Licitaciones.Application.Common`, eliminando una responsabilidad compartida mal ubicada. No agregó comportamiento; la suite local permaneció verde. |

#### Resultado de pruebas (HU-23)

La línea base previa al refactor estaba verde con 216 pruebas. Después del
refactor, la ejecución focalizada de HU-23 terminó con 12 correctas, 0 fallidas
y 0 omitidas. La suite completa local `dotnet test Licitaciones.sln` terminó con
216 correctas, 0 fallidas y 0 omitidas. Persisten dos advertencias `CS1998`
preexistentes en pruebas funcionales.

#### Pendientes y candidatos a Issues separadas

No se identificó trabajo adicional necesario para cumplir HU-23. El estado del
PR y la Issue se mantienen abiertos; no se documenta cierre, merge ni éxito de
CI para el commit de refactor local.

### HU-24 — Mensajería de éxito, advertencia y error

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-24 — Mensajería de éxito, advertencia y error | 2 | [#53](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/53) | Criterios cubiertos por pruebas en verde; la Issue permanece abierta y no se marca como completada ni se cierra desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-24 | Tiffany | Seidy | `6b15bed` |
| VERDE HU-24 | Seidy | Tiffany | `e6213df` |
| REFACTOR HU-24 | Tiffany | Seidy | Cambios locales sin commit |

Los roles conservan la asignación planificada (Tiffany Driver, Seidy Navigator)
y se reconstruyen a partir de la autoría alternada de los commits; Git conserva
la autoría del Driver, no evidencia independiente del rol Navigator.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

| Criterio de aceptación de la Issue #53 | Pruebas | Commits |
| --- | --- | --- |
| Una operación exitosa muestra un mensaje de confirmación (toast/alert). | `Operacion_Exitosa_EliminacionNivel_DebeMostrarAlertaConfirmacionEnDestino` y `Operacion_Exitosa_RegistroOferta_DebeMostrarAlertaConfirmacionEnListado` en `MensajeriaWebTests`. | ROJO `6b15bed`; VERDE `e6213df`; REFACTOR local sin commit. |
| Un error de negocio produce un mensaje específico y comprensible, no un stack trace. | `ErrorNegocio_TraslapeNiveles_DebeMostrarAlertaConMensajeEspecificoSinStacktrace` en `MensajeriaWebTests`. | ROJO `6b15bed`; VERDE `e6213df`; REFACTOR local sin commit. |

El PR [#64](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/64)
(`iteracion-3/hu-24-mensajeria` hacia `main`) está abierto como draft. Los
commits `6b15bed` y `e6213df` están publicados en la rama remota; el refactor
permanece local y todavía no forma parte del PR. No se atribuye un resultado de
CI al refactor local.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `6b15bed` — `test(web): cubrir criterios de mensajería de éxito, advertencia y error (HU-24)` | Agregó tres pruebas de integración HTTP con trait `HU-24` en `MensajeriaWebTests`: dos operaciones exitosas con redirección (eliminación de nivel y registro de oferta) y un error de negocio por traslape de rangos. Fallaron por comportamiento ausente: las vistas destino no mostraban la alerta de confirmación y los errores de negocio aparecían solo como texto del resumen de validación, sin componente `alert-danger`. CI fallido como es esperable en rojo (ejecución `32664841423`). Durante la fase se corrigió un fallo artificial (monto de oferta mayor al presupuesto sembrado) para que el rojo reflejara solo el comportamiento esperado. |
| VERDE | `e6213df` — `feat(web): implementar mensajería de éxito, advertencia y error (HU-24)` | Incorporó el parcial compartido `_Mensajes.cshtml` (alerta `alert-success` desde `TempData["MensajeExito"]` y resumen de validación dentro de `alert-danger`), su inclusión en las vistas destino de las redirecciones y el registro de un `HtmlEncoder` con soporte Latin-1 para los acentos. Filtro HU-24 con 3 correctas; suite completa en 219 verdes; CI en `success` (ejecución `32665845522`). |
| REFACTOR | Sin commit — cambios locales | Extendió `<partial name="_Mensajes" />` a las vistas que aún mantenían bloques duplicados (`Proveedores/Create`, `Proveedores/Edit`, `Ofertas/Create`, `TiposCambio/Create`, `TiposCambio/Index`, `Licitaciones/Index`), extrajo los usings de `HtmlEncoder`/`UnicodeRanges` en `Program.cs` y ajustó la prueba estructural `CreateView_DebeRenderizarMensajesDeValidacionDelNombre` para validar el resumen a través del parcial, preservando su intención. Sin comportamiento nuevo; suite completa en 219 verdes y `dotnet format --verify-no-changes` sin diferencias. |

#### Resultado de pruebas (HU-24)

La línea base previa al incremento estaba verde con 216 pruebas. Tras el ROJO,
la ejecución focalizada de HU-24 terminó con 3 fallidas y 0 correctas. Después
del VERDE, el filtro HU-24 terminó con 3 correctas, 0 fallidas y 0 omitidas, y
la suite completa `dotnet test Licitaciones.sln` con 219 correctas, 0 fallidas
y 0 omitidas. El refactor local mantuvo la suite en 219 verdes, con build sin
errores y formato verificado.

#### Pendientes y candidatos a Issues separadas

- El POST exitoso de `Licitaciones/Create` fija `TempData["MensajeExito"]`,
  pero la vista correspondiente aún no incluye el parcial `_Mensajes`, por lo
  que esa confirmación no llega a mostrarse.
- La edición exitosa de un proveedor fija `TempData["MensajeExito"]` y
  redirige a `Details`, vista que tampoco renderiza el mensaje.
- El título de HU-24 menciona la variante de advertencia, pero ningún flujo
  produce todavía `TempData["MensajeAdvertencia"]`; el parcial actual cubre
  éxito y error.

Estos puntos se reportan como candidatos a Issues separadas; no se ocultan
dentro de esta historia. La Issue #53 permanece abierta.

### HU-25 — Formato monetario y cultural es-CR

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-25 — Formato monetario y cultural es-CR | 1 | [#54](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/54) | Criterio cubierto por pruebas en verde; la Issue permanece abierta y no se marca como completada ni se cierra desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-25 | Seidy | Tiffany | `857f458` |
| VERDE HU-25 | Tiffany | Seidy | `f3ff76e` |
| REFACTOR HU-25 | Seidy | Tiffany | `4fd4175` |

Los roles conservan la asignación planificada (Seidy Driver, Tiffany Navigator)
y se reconstruyen a partir de la autoría alternada de los commits; Git conserva
la autoría del Driver, no evidencia independiente del rol Navigator.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

La Issue #54 se contrastó con `docs/historias-usuario.md` antes de programar:
título, prioridad Baja, estimación 1 SP y el criterio único coinciden.

| Criterio de aceptación de la Issue #54 | Pruebas | Commits |
| --- | --- | --- |
| Un monto en CRC presentado en cualquier vista usa separador de miles y formato es-CR (ej. ₡1.500.000,00). | `Listado_Licitaciones_DebePresentarPresupuestoConFormatoEsCR`, `Listado_Ofertas_DebePresentarMontoConFormatoEsCR` y `Listado_NivelesAprobacion_DebePresentarMontosConFormatoEsCR` en `FormatoMonetarioWebTests`. | ROJO `857f458`; VERDE `f3ff76e`; REFACTOR `4fd4175`. |

El PR [#65](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/65)
(`iteracion-3/hu-25-formato-es-cr` hacia `main`) está abierto como draft. Los
commits `857f458` y `f3ff76e` están publicados en la rama remota; el refactor
`4fd4175` permanece local y todavía no forma parte del PR. No se atribuye
resultado de CI al commit local del refactor.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `857f458` — `test(web): cubrir criterios de formato monetario y cultural es-cr (HU-25)` | Agregó tres pruebas de integración HTTP con trait `HU-25` en `FormatoMonetarioWebTests`, sobre PostgreSQL real y vistas MVC servidas por `WebApplicationFactory`: presupuesto de licitación, monto de oferta y montos de un nivel de aprobación, cada uno con su valor esperado exacto (`₡1.500.000,00`, `₡1.250.500,00`, `₡23.456.789,00` y `₡24.654.321,00`). Fallaron por comportamiento ausente: las vistas renderizaban `.ToString("N2")` sin cultura ni símbolo colón. CI fallido como es esperable en rojo (ejecución `32669889839`). Durante la fase se corrigió un fallo artificial (el rango sembrado traslapaba el nivel Directivo activo) desactivando temporalmente el nivel 3 y restaurándolo al final, igual que en `MensajeriaWebTests`. |
| VERDE | `f3ff76e` — `feat(web): implementar formato monetario y cultural es-cr (HU-25)` | Incorporó el helper `FormatoMonetario` con métodos de extensión `Dinero()` sobre `decimal` y `decimal?` usando una cultura `es-CR` clonada con separador de miles `.` y congelada con `CultureInfo.ReadOnly`; amplió el `HtmlEncoder` registrado con `UnicodeRanges.CurrencySymbols` para que ₡ no se escape; y aplicó `.Dinero()` en los listados de licitaciones, ofertas y niveles de aprobación. Filtro HU-25 con 3 correctas; suite completa en 222 verdes; CI en `success` (ejecución `32673569441`). |
| REFACTOR | `4fd4175` — `refactor(web): simplificar implementacion de HU-25` | Mejoras de legibilidad y consistencia solo en las pruebas: reemplazó los escapes `\u20A1` por el literal ₡ en las constantes de montos esperados e importó `Licitaciones.Domain.Aprobaciones` eliminando la calificación completa de `NivelAprobacion.Crear`, alineándose con `MensajeriaWebTests`. El código de producción se evaluó sin cambios justificados (helper único, sin duplicación, controladores delgados). Sin comportamiento nuevo; filtro HU-25 en 3 correctas, suite completa en 222 verdes y `dotnet format --verify-no-changes` sin diferencias. Permanece local sin ejecución de CI registrada. |

#### Resultado de pruebas (HU-25)

La línea base previa al incremento estaba verde con 219 pruebas. Tras el ROJO,
la ejecución focalizada `dotnet test …IntegrationTests.csproj --filter
"HU=HU-25"` terminó con 3 fallidas y 0 correctas. Después del VERDE, el mismo
filtro terminó con 3 correctas, 0 fallidas y 0 omitidas, y la suite completa
`dotnet test Licitaciones.sln` con 222 correctas, 0 fallidas y 0 omitidas. El
refactor mantuvo la suite en 222 verdes, con formato verificado.

#### Pendientes y candidatos a Issues separadas

- `Views/NivelesAprobacion/Delete.cshtml` sigue presentando los montos mínimo y
  máximo con `.ToString("N2")`; extender `.Dinero()` allí completaría el
  criterio "cualquier vista" para vistas fuera de los listados.
- El valor del tipo de cambio se muestra con `.ToString("N4")` en
  `TiposCambio/Index`: no es un monto en colones, por lo que se dejó sin cambio.
- Duplicación del arranque de `WebApplicationFactory` entre clases de
  IntegrationTests; candidato a un builder compartido.
- Pruebas unitarias dedicadas al helper `FormatoMonetario` si se desea
  cobertura fina adicional.

Estos puntos se reportan como candidatos a Issues separadas; no se ocultan
dentro de esta historia. La Issue #54 permanece abierta.

### HU-26 — Exponer API REST con DTOs y versionado

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-26 — Exponer API REST con DTOs y versionado | 8 | [#55](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/55) | Criterios cubiertos por pruebas en verde; la Issue permanece abierta y no se marca como completada ni se cierra desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-26 | Tiffany | Seidy | `9611c8d` |
| VERDE HU-26 | Seidy | Tiffany | `7db6e80` |
| REFACTOR HU-26 | Tiffany | Seidy | Cambios locales sin commit |

Los roles conservan la asignación planificada (Tiffany Driver, Seidy Navigator)
y se reconstruyen a partir de la autoría alternada de los commits; Git conserva
la autoría del Driver, no evidencia independiente del rol Navigator.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

La Issue #55 se contrastó con `docs/historias-usuario.md` antes de programar:
título, prioridad Alta, estimación 8 SP y los cinco criterios coinciden. Dos
desviaciones de trazabilidad se detectaron y se reportan sin corregirlas en
esta fase: la rama real (`iteracion-3/hu-26-api-rest`) difiere de la prevista
(`iteracion-3/hu-26-api-rest-versionada`) y el commit VERDE quedó referenciado
como `refs #56` en lugar de `refs #55`.

| Criterio de aceptación de la Issue #55 | Pruebas | Commits |
| --- | --- | --- |
| Cualquier endpoint retorna DTOs específicos, nunca entidades EF Core. | Suite HTTP previa por módulo (HU-10/14/15/16/17) con `[ProducesResponseType]` tipado y aserciones sobre DTO. | Evidencia previa; sin commits nuevos de HU-26 para este criterio. |
| La ruta base incluye versión (`/api/v1/...`). | Rutas `/api/v1/{modulo}` verificadas en los cinco controladores API. | Ídem. |
| CRUD retorna códigos HTTP correctos (200, 201, 204, 400, 404, 409, 422 y 500 controlado). | Pruebas previas por módulo más las cinco de `ContratoApiRestHttpTests`; `GetPorId_Inexistente_DebeResponder404` retiquetada a `HU-26` con aserción reforzada. | ROJO `9611c8d`; VERDE `7db6e80`; REFACTOR local sin commit. |
| Cualquier error usa ProblemDetails (título, estado, detalle seguro, código de error, correlación), sin stack traces ni rutas internas. | `Error_BadRequest_…`, `Error_Conflicto_Duplicado_…`, `Error_NoEncontrado_…`, `Error_Negocio_PresupuestoSuperado_…` e `Error_Interno_NoControlado_…` en `ContratoApiRestHttpTests`. | ROJO `9611c8d`; VERDE `7db6e80`; REFACTOR local sin commit. |
| Listados con paginación, filtrado y ordenamiento vía query params. | Pruebas previas de proveedores, licitaciones y ofertas; HU-26 no duplicó escenarios de listado. | Evidencia previa; sin commits nuevos de HU-26 para este criterio. |

El PR [#66](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/66)
(`iteracion-3/hu-26-api-rest` hacia `main`) está abierto como draft. Los
commits `9611c8d` y `7db6e80` están publicados en la rama remota; el refactor
permanece local y todavía no forma parte del PR. No se atribuye un resultado
de CI al refactor local.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `9611c8d` — `test(api): cubrir criterios de exponer api rest con dtos y versionado (HU-26)` | Agregó `ContratoApiRestHttpTests` (namespace `Hu26`, trait `HU-26`) con cinco pruebas HTTP reales que fijan el contrato transversal de errores: 400 por nombre inválido, 409 por proveedor duplicado, 404 de recurso inexistente, 422 por oferta sobre presupuesto y 500 provocado saboteando `ConsultarProveedorService`. Todas exigen `application/problem+json` con título, estado, detalle seguro, `codigoError` y `correlacionId`; el caso interno además rechaza stack traces y rutas del proyecto. Fallaron por comportamiento ausente: faltaban las dos extensiones del contrato, el 404 respondía sin cuerpo y el 500 sin detalle. CI fallido como es esperable en rojo (ejecución `32676486243`). |
| VERDE | `7db6e80` — `feat(api): implementar exponer api rest con dtos y versionado (HU-26)` | Registró `FabricaProblemDetailsApi` como `ProblemDetailsFactory` personalizado, centralizó las respuestas de error de controladores en `RespuestaProblema` y añadió un manejador global que mapea `DomainException` a 422 con código `regla_negocio_no_procesable` y lo no previsto a un 500 controlado con código `error_interno`. Retiquetó la prueba existente `GetPorId_Inexistente_DebeResponder404` de `HU-09` a `HU-26` reforzando su aserción al nuevo cuerpo. Filtro HU-26 con 6 correctas; suite completa en 227 verdes; CI en `success` (ejecución `32677819388`). |
| REFACTOR | Sin commit — cambios locales | Extrajo `ContratoProblemasApi` como única fuente de las claves `codigoError`/`correlacionId` y de la aplicación de extensiones, delegaron en ella `FabricaProblemDetailsApi` y `RespuestaProblema`, y el manejador de `Program.cs` pasó a construir el problema mediante la fábrica registrada (`ProblemDetailsFactory.CreateProblemDetails`) eliminando la construcción manual duplicada; además retiró el modificador `partial` innecesario de `OpcionesJsonHttp`. Sin comportamiento nuevo: filtro HU-26 con 6 correctas, suite completa en 227 verdes y `dotnet format --verify-no-changes` sin diferencias. Los cinco wrappers privados `CrearProblema` de una línea se conservaron porque sustituirlos por un método de extensión tocaría unas veinte llamadas por una ganancia marginal. |

#### Resultado de pruebas (HU-26)

La línea base previa al incremento estaba verde con 222 pruebas. Tras el ROJO,
la ejecución focalizada terminó con 5 fallidas y 0 correctas. Después del
VERDE, el filtro HU-26 terminó con 6 correctas, 0 fallidas y 0 omitidas, y la
suite completa `dotnet test Licitaciones.sln` con 227 correctas, 0 fallidas y
0 omitidas. El refactor local mantuvo la suite en 227 verdes, con build sin
errores y formato verificado.

#### Pendientes y candidatos a Issues separadas

- El mensaje del commit VERDE `7db6e80` dice `refs #56` cuando debe decir
  `refs #55`; requiere corrección de trazabilidad.
- La rama prevista era `iteracion-3/hu-26-api-rest-versionada` y la real es
  `iteracion-3/hu-26-api-rest`; la Issue y el backlog deben alinearse.
- `WeatherForecastController` y `WeatherForecast` son residuos de plantilla en
  `Licitaciones.Api`; su eliminación es candidata a limpieza separada.
- Los cinco controladores mantienen un wrapper privado `CrearProblema` de una
  línea sobre `RespuestaProblema.Crear`; convertirlo en método de extensión de
  `ControllerBase` tocaría unas veinte llamadas y se dejó fuera por ganancia
  marginal.

Estos puntos se reportan como candidatos a Issues separadas; no se ocultan
dentro de esta historia. La Issue #55 permanece abierta.

### HU-27 — Documentación interactiva OpenAPI/Swagger

#### Estado

| Historia | SP | Issue | Estado |
| --- | ---: | --- | --- |
| HU-27 — Documentación interactiva OpenAPI/Swagger | 2 | [#56](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/56) | Criterios cubiertos por pruebas en verde; la Issue permanece abierta y no se marca como completada ni se cierra desde esta fase. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-27 | Seidy | Tiffany | `3af0427` |
| VERDE HU-27 | Tiffany | Seidy | `b790880` |
| REFACTOR HU-27 | Seidy | Tiffany | Commit local `14a8421` sin publicar |

Los roles conservan la asignación planificada (Seidy Driver, Tiffany Navigator)
y se reconstruyen a partir de la autoría alternada de los commits; Git conserva
la autoría del Driver, no evidencia independiente del rol Navigator.

#### Trazabilidad Issue → criterios → pruebas → commits → PR

La Issue #56 se contrastó con `docs/historias-usuario.md` antes de programar:
título, prioridad Media, estimación 2 SP y los dos criterios coinciden. A
diferencia de HU-26, la rama real coincide con la prevista
(`iteracion-3/hu-27-swagger`) y los commits usan `refs #56` correctamente.

| Criterio de aceptación de la Issue #56 | Pruebas | Commits |
| --- | --- | --- |
| `/swagger` muestra la documentación generada con todos los endpoints, esquemas de request/response y ejemplos. | Las cuatro pruebas de `DocumentacionSwaggerHttpTests`: interfaz servida en `/swagger`, documento con las 14 rutas del dominio, esquemas (`ProveedorDto`, `LicitacionDto`, `OfertaDto`, `TipoCambioDto`, `ProblemDetails`, `ValidationProblemDetails`) con cuerpo de solicitud JSON y ejemplos por esquema. | ROJO `3af0427`; VERDE `b790880`; REFACTOR `14a8421` local. |
| `/docs/api.md` documenta endpoints, contratos de request/response, ejemplos y errores, y referencia una colección reproducible que existe. | Las dos pruebas de `DocumentacionApiMarkdownTests`: contenido de `api.md` (recursos, errores con `ProblemDetails`/`codigoError`/`correlacionId`, bloques `json`/`http`) y existencia más cobertura de recursos de la colección referenciada (`docs/api.http`). | Ídem. |

El PR [#67](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/67)
(`iteracion-3/hu-27-swagger` hacia `main`) está abierto como draft. Los commits
`3af0427` y `b790880` están publicados en la rama remota; el refactor
`14a8421` permanece local y todavía no forma parte del PR ni tiene ejecución
de CI registrada. El ROJO falló en CI como es esperable (ejecución
`32682545858`); tras el VERDE la ejecución `32684426351` terminó en `success`.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `3af0427` — `test(api): cubrir criterios de documentación interactiva openapi/swagger (HU-27)` | Agregó seis pruebas con trait `HU-27`: cuatro HTTP reales en `DocumentacionSwaggerHttpTests` (namespace `Hu27`) que exigen Swagger UI servida en `/swagger`, documento OpenAPI con las 14 rutas del dominio, esquemas request/response y ejemplos; dos unitarias en `DocumentacionApiMarkdownTests` sobre `docs/api.md`. Resultado observado: 4 HTTP fallidas (404: sin Swagger UI ni swagger.json) y 1 de 2 unitarias fallida porque la colección reproducible no existía ni estaba referenciada; la prueba de contenido pasó legítimamente porque `api.md` ya documentaba recursos y errores desde iteraciones previas. CI fallido como es esperable en rojo (ejecución `32682545858`). |
| VERDE | `b790880` — `feat(api): implementar documentación interactiva openapi/swagger (HU-27)` | Incorporó Swashbuckle 7.2.0 (`SwaggerGen`, `SwaggerUI`, `Swagger`), `GenerateDocumentationFile` con `NoWarn CS1591`, registro `AddSwaggerGen` (documento v1, comentarios XML vía rutaXml, `SchemaFilter<EjemplosEsquemasFiltro>`) y middleware `UseSwagger`/`UseSwaggerUI` solo en Development. `EjemplosEsquemasFiltro` aporta ejemplos para 11 esquemas (4 DTO de respuesta y 7 contratos de solicitud). `docs/api.md` ganó la sección «Documentación interactiva (HU-27)» y nació la colección reproducible `docs/api.http` cubriendo los cinco recursos. Filtro HU-27 con 6 correctas; suite completa en 233 verdes; CI en `success` (ejecución `32684426351`). |
| REFACTOR | `14a8421` — `refactor(api): simplificar implementacion de HU-27` (local, sin push) | Sustituyó la cadena ternaria de 11 ramas de `EjemplosEsquemasFiltro.Apply` por un diccionario estático `EjemplosPorTipo` (`Type` → fábrica de ejemplo) y alineó nombres ambiguos con sus contratos (`EjemploNivelAprobacion` → `EjemploGuardarNivelAprobacion`, `EjemploTipoCambioSolicitud` → `EjemploGuardarTipoCambio`). Descartó extraer el bloque `AddSwaggerGen` de `Program.cs` por ser abstracción especulativa frente al estilo inline existente. Sin comportamiento nuevo: suite en 233 verdes y formato sin diferencias. Sin push ni CI registrado. |

#### Resultado de pruebas (HU-27)

La línea base previa al incremento estaba verde con 227 pruebas. En el ROJO,
las 4 pruebas HTTP nuevas fallaron por comportamiento ausente y 1 unitaria
falló por la colección faltante. Tras el VERDE, el filtro HU-27 terminó con 6
correctas, 0 fallidas y 0 omitidas, y la suite completa
`dotnet test Licitaciones.sln` quedó en 233 correctas, 0 fallidas y 0 omitidas.
El refactor local mantuvo la suite en 233 verdes, con build sin errores y
formato verificado:

| Proyecto | Superadas | Fallidas | Omitidas |
| --- | ---: | ---: | ---: |
| `Licitaciones.UnitTests` | 85 | 0 | 0 |
| `Licitaciones.IntegrationTests` | 132 | 0 | 0 |
| `Licitaciones.FunctionalTests` | 16 | 0 | 0 |
| **Total ejecutado** | **233** | **0** | **0** |

#### Pendientes y candidatos a Issues separadas

- La fábrica `CrearApiFactory` está duplicada entre `ContratoApiRestHttpTests`
  (HU-26) y `DocumentacionSwaggerHttpTests` (HU-27); consolidarla tocaría
  pruebas de otra historia.
- El commit de refactor `14a8421` permanece local: ni el PR #67 ni CI lo
  incluyen todavía.
- Los ajustes de documentación de esta fase (deduplicación en `api.md`,
  encabezado de uso y demostración de error en `api.http`) permanecen locales,
  sin commit.

Estos puntos se reportan como candidatos a Issues separadas; no se ocultan
dentro de esta historia. La Issue #56 permanece abierta.

## Cierre de la Iteración 3

Cierre documental registrado el 24 de agosto de 2026 sobre el commit `666f175`
de `main`, desde la rama `iteracion-3/docs-cierre`, sin modificar código. Las
fusiones se verificaron con `git log main --merges`; el estado de Issues,
Pull Requests y GitHub Actions se consultó en la API pública de GitHub durante
este cierre; la suite completa y el formato se ejecutaron localmente sobre ese
mismo commit. Las entradas por historia de esta iteración conservan el estado
de cada fase tal como se registró (Issues abiertas, PRs en draft, refactors
locales sin publicar); este cierre consolida el estado final verificado sobre
`main`.

### Verificación de fusión en `main`

Las diez historias seleccionadas están fusionadas en `main` mediante los PR
#58 a #67, cada uno con CI verde en su commit de fusión:

| PR | Commit de fusión | Historia | SP | Ejecución de CI en `main` | Resultado |
| --- | --- | --- | ---: | --- | --- |
| #58 | `73d399a` | HU-18 — Administrar niveles de aprobación | 5 | `32560841685` | success |
| #59 | `8564387` | HU-19 — Tipo de cambio y conversión CRC/USD | 5 | `32610626534` | success |
| #60 | `c9e36e5` | HU-20 — Landing page informativa | 3 | `32617145891` | success |
| #61 | `6abc37c` | HU-21 — Menú de navegación global | 2 | `32646374137` | success |
| #62 | `39d755a` | HU-22 — Modo claro/oscuro persistente | 2 | `32653636518` | success |
| #63 | `95ba997` | HU-23 — CRUD completo desde la interfaz web | 8 | `32661616283` | success |
| #64 | `20d8d9d` | HU-24 — Mensajería de éxito, advertencia y error | 2 | `32669007316` | success |
| #65 | `09fb478` | HU-25 — Formato monetario y cultural es-CR | 1 | `32675111713` | success |
| #66 | `df31428` | HU-26 — Exponer API REST con DTOs y versionado | 8 | `32680134397` | success |
| #67 | `666f175` | HU-27 — Documentación interactiva OpenAPI/Swagger | 2 | `32688275482` | success |

La planificación de la iteración entró previamente mediante el PR #57
(`6b8e51b`, ejecución `32529040965`, success), sin puntos de historia.

### Trazabilidad Issues ↔ historias ↔ PRs

Cada historia tiene exactamente una Issue asociada, creada durante el Planning
Game y no retrospectivamente: HU-18→[#47](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/47),
HU-19→[#48](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/48),
HU-20→[#49](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/49),
HU-21→[#50](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/50),
HU-22→[#51](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/51),
HU-23→[#52](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/52),
HU-24→[#53](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/53),
HU-25→[#54](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/54),
HU-26→[#55](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/55) y
HU-27→[#56](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/56).

Ninguna Issue permanece abierta y ninguna se cerró de forma prematura: cada
cierre ocurrió inmediatamente después de la fusión de su PR (por ejemplo, #47
se cerró tres minutos después del merge `73d399a`; #56, un minuto después de
`666f175`). El estado CLOSED no se usó como única evidencia de cumplimiento:
cada historia se validó además contra sus pruebas con trait propio en verde,
sus commits rojo/verde/refactor, su documentación por historia y el CI success
de su fusión, según el detalle de las tablas DoD siguientes.

Observaciones de trazabilidad registradas sin corregir el historial:

1. El commit VERDE de HU-26 (`7db6e80`) dice `refs #56` cuando debe decir
   `refs #55`.
2. Cuatro ramas reales difieren de las previstas en el Planning Game:
   HU-21 (`iteracion-3/hu-21-navegacion` frente a `…navegacion-global`),
   HU-22 (`…modo-claro-oscuro` frente a `…tema-claro-oscuro`),
   HU-23 (`…crud-web` frente a `…crud-web-completo`) y
   HU-26 (`…api-rest` frente a `…api-rest-versionada`). Sin impacto en
   alcance, criterios ni contenido.
3. La entrada histórica de HU-21 en esta bitácora afirma «No se registró PR de
   HU-21»; quedó desactualizada frente al PR #61 fusionado y se corrige por
   medio de este cierre.

### Definition of Done por historia

La evaluación aplica los criterios verificables de `plan-xp.md`. Las diez
historias exponen MVC o API, por lo que el criterio de recorridos HTTP reales
aplica a todas y está cubierto en todas.

| Historia | SP | Cumple DoD | Observación verificada |
| --- | ---: | --- | --- |
| HU-18 | 5 | Sí | CRUD web y API de niveles con resolución de aprobador; recorridos HTTP reales y persistencia probada hasta PostgreSQL. |
| HU-19 | 5 | Sí | Tipo de cambio activo administrable y conversión CRC/USD integrada a ofertas y detalle; reglas en Application/Domain y pruebas hasta PostgreSQL. |
| HU-20 | 3 | Sí | Landing informativa con contenido del flujo del sistema; verificada por prueba funcional HTTP real. |
| HU-21 | 2 | Sí | Navegación global con resaltado de página activa y enlace a Swagger; seis casos funcionales HTTP. |
| HU-22 | 2 | Sí | Tema claro/oscuro persistente en `localStorage` sin parpadeo inicial; cinco casos funcionales HTTP. Salvedad UX menor: el ícono del control no refleja el tema activo. |
| HU-23 | 8 | Sí | CRUD web completo de los cinco módulos; doce pruebas CRUD por HTTP real con PostgreSQL. |
| HU-24 | 2 | Sí | Parcial `_Mensajes` renderizado y verificado por tres casos HTTP. Salvedad UX registrada: los mensajes no son visibles tras crear licitación ni tras editar proveedor; los criterios de la Issue quedaron cubiertos por pruebas y el ajuste visual se lleva a la siguiente iteración. |
| HU-25 | 1 | Sí | Helper `FormatoMonetario` con ₡ y cultura es-CR en los montos de listados; encoder ampliado a símbolos monetarios. Salvedad menor: `NivelesAprobacion/Delete` conserva `ToString("N2")`. |
| HU-26 | 8 | Sí | API REST `/api/v1` con DTOs, códigos correctos y contrato transversal ProblemDetails (`codigoError`/`correlacionId`) sin stack traces. |
| HU-27 | 2 | Sí | Swagger UI y documento OpenAPI con endpoints, esquemas y ejemplos en Development; `docs/api.md` referencia la colección reproducible `docs/api.http`. |

El build Release, `dotnet format --verify-no-changes` y la suite completa sin
errores ni omitidas quedan cubiertos por el CI verde de cada fusión y se
re-verificaron localmente en este cierre: 233 correctas, 0 fallidas y 0
omitidas, formato sin diferencias. La documentación por módulo quedó alineada
en el cierre de cada historia. El último criterio del DoD (pequeña liberación
etiquetada) se cumple al cerrar la iteración: ver «Pequeña liberación».

### Velocidad planificada frente a observada

- Velocidad planificada de referencia: **36 SP** (`plan-xp.md`).
- Alcance seleccionado en el Planning Game: **38 SP**, con la diferencia de
  +2 SP registrada como riesgo de planificación al iniciar.
- Alcance fusionado en `main`: **38 SP** (las diez historias).
- Velocidad observada al cierre: **38 SP**, contando las diez historias que
  cumplen la Definition of Done.
- Desviación: **+2 SP** frente a la referencia planificada —el riesgo
  declarado se materializó y absorbió sin recortar alcance— y **±0 SP**
  frente al alcance seleccionado.

### Ciclos TDD y refactorizaciones

Las diez historias siguieron ciclos rojo–verde–refactor con commits separados
y evidencia CI: cada ROJO publicó pruebas que fallaron por comportamiento
ausente (por ejemplo `9611c8d` en HU-26 y `3af0427` en HU-27, con ejecuciones
fallidas esperables) y cada VERDE quedó en success (por ejemplo `7db6e80` con
`32677819388` y `b790880` con `32684426351`). Los rojos unitarios de HU-27
pasaron 1 de 2 porque `docs/api.md` ya documentaba recursos y errores de
iteraciones previas; la parte faltante (colección reproducible referenciada)
sí falló, y así quedó registrado en la entrada de la historia.

Refactorizaciones relevantes del cierre de cada ciclo (detalle por historia en
las secciones anteriores):

- HU-18: renombró `ResolverNivelAprobacion` a `ResolverAsync` y alineó el
  helper del controlador con la convención `CrearProblema`.
- HU-19: centralizó el par USD/CRC en constantes del dominio y consolidó
  `ITipoCambioRepository.ObtenerActivoAsync` como única fuente del activo.
- HU-22: eliminó la aplicación inicial del tema duplicada entre layout y
  `site.js`.
- HU-23: movió `PaginaResultado<T>` de `Contracts.Proveedores` a
  `Application.Common` como contrato compartido.
- HU-26: extrajo `ContratoProblemasApi` como única fuente del contrato de
  errores y reutilizó la fábrica registrada en el manejador global.
- HU-27: sustituyó la cadena ternaria de 11 ramas del filtro de ejemplos por
  un diccionario estático tipo→ejemplo.

### Participación Seidy/Tiffany

La iteración acumula 57 commits entre `5f731fb` y `666f175` (incluye las dos
fusiones documentales de regularización de la Iteración 2, PRs #46 y #28):
Seidy Oporta firma 28 (22 directos más 6 merges como `Seidy06`) y Tiffany
Alfaro 29 (22 directos más 7 merges como `tiffanyyulieth08`). La autoría
alternó en las fases rojo, verde, refactor y documental de cada historia y las
tablas por historia registran la rotación Driver/Navigator. Git conserva la
autoría del Driver; el rol Navigator se reconstruye a partir del trabajo
coordinado de la pareja, sin atribuir sesiones sin evidencia.

### Integración continua

El workflow mantiene restore, `dotnet format --verify-no-changes`, build
Release y suite completa con PostgreSQL 16 como servicio, en push y
pull_request hacia `main`. Los once pushes al tronco durante el ciclo
(planificación más las diez historias) terminaron en success, con las
ejecuciones listadas en la tabla de fusiones. El workflow sigue sin medir
cobertura ni construir imágenes Docker: es alcance explícito de la
Iteración 4.

### Resultado de la demostración

No existe en el repositorio un acta de demostración ni una aprobación firmada
del cliente, por lo que no se registra retroalimentación externa. Lo
demostrable y reproducible del incremento es:

- Por HTTP real: administrar niveles de aprobación (web y API), registrar el
  tipo de cambio activo y ver la conversión CRC/USD aplicada a ofertas y
  detalle de ofertas, operar el CRUD completo de los cinco módulos desde la
  interfaz web, consumir la API REST `/api/v1` con DTOs y contrato de errores
  ProblemDetails, y explorar la documentación interactiva Swagger con esquemas
  y ejemplos.
- En experiencia de usuario: landing informativa, navegación global con
  enlace a Swagger y tema claro/oscuro persistente, con mensajería de
  resultado y montos en colones con cultura es-CR.
- Salvedades visibles registradas como ajustes: mensajes invisibles en dos
  flujos de HU-24 e ícono de tema estático en HU-22.

La suite que respalda esta demostración es la registrada en `pruebas.md`:
233 pruebas superadas, 0 fallidas y 0 omitidas.

### Retroalimentación y ajustes para la Iteración 4

Sin acta del cliente, los ajustes se derivan exclusivamente de brechas
verificables en el código fusionado:

1. Hacer visible la mensajería tras crear una licitación y tras editar un
   proveedor (redirecciones actuales sin parcial `_Mensajes`).
2. Dotar de productores reales a la variante de advertencia de la mensajería.
3. Reflejar el tema activo en el ícono del control de tema.
4. Aplicar el formato ₡ es-CR a los montos de `NivelesAprobacion/Delete`.
5. Eliminar los residuos de plantilla `WeatherForecastController` y
   `WeatherForecast` de `Licitaciones.Api`.
6. Consolidar duplicaciones de pruebas: la fábrica `CrearApiFactory` entre
   HU-26/HU-27 y la colección `PaginasDelSitio` entre HU-21/HU-22.
7. Corregir trazabilidad documental: referencia `refs #56` del commit VERDE
   de HU-26 y nombres de ramas previstas frente a reales en el backlog.
8. Desarrollar el alcance planificado de la Iteración 4 (HU-28 a HU-37):
   pruebas complementarias, Docker y Kubernetes, integración continua
   ampliada, documentación final y etiquetado de la entrega.

### Pequeña liberación

La etiqueta `v0.3.0` no existe todavía (`git tag -l` muestra solo `v0.1.0` y
`v0.2.0`). Se creará después de fusionar esta rama de cierre documental,
identificando el incremento HU-18 a HU-27 completas (38 SP observados), sin
salvedades de exposición pendientes.

## Corrección posterior a la auditoría final de Iteración 2

Driver: Tiffany. Navigator/responsable: Seidy. La auditoría detectó que HU-11 y
HU-12 no tenían superficie HTTP y que HU-13/HU-17 no completaban paginación,
filtro y ordenamiento. Se agregaron pruebas unitarias y HTTP sobre PostgreSQL
real, luego las operaciones Domain/Application/API y los contratos paginados.
No se agregaron contenedores: las pruebas siguen usando una sola colección y
fixture PostgreSQL compartida. Los cambios permanecen sin commit mientras se
realiza la verificación final; no se atribuye CI remoto antes de publicarlos.

## Regularización retrospectiva de trazabilidad — Iteración 2

Esta regularización se realizó después de que la Iteración 2 ya había sido
implementada, documentada, auditada y cerrada, y después de la creación de la
etiqueta `v0.2.0`. Posteriormente se detectó que HU-10 a HU-17 no estaban
asociadas correctamente con GitHub Issues por un problema de coordinación.

Se crearon los Issues #29 a #36 como referencias retrospectivas y se añadieron
comentarios de relación a los PR #19 a #26. El PR #28 quedó relacionado además
como evidencia complementaria de las correcciones finales de HU-11, HU-12,
HU-13 y HU-17. Los Issues no se presentan como artefactos anteriores a la
implementación y no se añadieron referencias artificiales a commits históricos.

| Historia | Issue retrospectivo | PR principal | Rama histórica |
| --- | --- | --- | --- |
| HU-10 — Crear licitación | [#29](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/29) | [#19](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/19) | `iteracion-2/hu-10-crear-licitacion` |
| HU-11 — Publicar licitación | [#30](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/30) | [#20](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/20) | `iteracion-2/hu-11-publicar-licitacion` |
| HU-12 — Editar y cerrar licitación | [#31](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/31) | [#21](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/21) | `iteracion-2/hu-12-editar-cerrar-licitacion` |
| HU-13 — Listar y consultar licitaciones | [#32](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/32) | [#22](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/22) | `iteracion-2/hu-13-consultar-licitaciones` |
| HU-14 — Registrar oferta | [#33](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/33) | [#23](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/23) | `iteracion-2/hu-14-registrar-oferta` |
| HU-15 — Rechazar y auditar ofertas inválidas | [#34](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/34) | [#24](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/24) | `iteracion-2/hu-15-rechazar-ofertas` |
| HU-16 — Calcular mejor oferta y clasificación de ahorro | [#35](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/35) | [#25](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/25) | `iteracion-2/hu-16-mejor-oferta` |
| HU-17 — Listar y consultar ofertas | [#36](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/36) | [#26](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/26) | `iteracion-2/hu-17-consultar-ofertas` |

No se reescribieron commits, ramas ni Pull Requests; no se repitió el cierre de
la iteración. El objeto anotado de `v0.2.0` y su commit objetivo permanecen
intactos.

## Iteración 2 — Ciclo de licitaciones y ofertas

**Estado: CERRADA — iniciada el 18 y cerrada documentalmente el 20 de agosto
de 2026. La pequeña liberación `v0.2.0` queda pendiente de etiquetado después
de fusionar la rama de cierre; la etiqueta aún no existe en el repositorio.**

### Objetivo

Completar el ciclo de una licitación —crearla, publicarla y cerrarla— y el
registro o rechazo explícito de ofertas mediante reglas de negocio, incluyendo
la determinación de la mejor oferta y su clasificación de ahorro.

### Planning Game de inicio

Se confirma el alcance HU-10 a HU-17 definido en `plan-xp.md`. Las prioridades
y estimaciones se conservan sin reestimarlas en este inicio formal.

| Orden | Historia | Prioridad | SP | Dependencias para el incremento | Estado inicial |
| ---: | --- | --- | ---: | --- | --- |
| 1 | HU-10 — Crear licitación | Alta | 5 | Base de dominio, persistencia y reloj de HU-02 a HU-05. | Seleccionada; no terminada |
| 2 | HU-11 — Publicar licitación | Alta | 3 | HU-10, porque solo puede publicarse una licitación creada en `Borrador`. | Seleccionada; no terminada |
| 3 | HU-14 — Registrar oferta | Alta | 5 | HU-11, HU-06 y HU-05: requiere licitación publicada, proveedor y reloj inyectable. | Terminada. |
| 4 | HU-12 — Editar y cerrar licitación | Alta | 5 | HU-10 y HU-14 para comprobar las restricciones de edición frente a ofertas existentes. | Refactor completado; sin endpoints HTTP ni DI. |
| 5 | HU-15 — Rechazar y auditar ofertas inválidas | Alta | 3 | HU-14 y HU-12 para verificar duplicidad, exceso de presupuesto, vencimiento e inmutabilidad tras el cierre. | Terminada. |
| 6 | HU-16 — Calcular mejor oferta y clasificación de ahorro | Alta | 5 | HU-14 para disponer de ofertas válidas. El nivel de aprobación correspondiente depende de HU-18, planificada para la Iteración 3. | Terminada. |
| 7 | HU-13 — Listar y consultar licitaciones | Media | 3 | HU-12 y HU-16 para mostrar estado efectivo y mejor oferta. El nivel de aprobación depende de HU-18. | Terminada. |
| 8 | HU-17 — Listar y consultar ofertas | Media | 2 | HU-14 y HU-16. Reutiliza el tipo de cambio activo existente para presentación en USD; la administración y fecha del tipo de cambio permanecen en HU-19. | Terminada. |
|  | **Total seleccionado** |  | **31** | **26 SP de prioridad alta y 5 SP de prioridad media.** | **Velocidad observada no registrada** |

El orden prioriza primero el recorrido ejecutable crear → publicar → ofertar →
cerrar; después fija los rechazos, el cálculo y las consultas. Las dependencias
con HU-18 y HU-19 se registran como límites conocidos del catálogo y no se
consideran satisfechas ni autorizan a adelantar esas historias.

### Programación en pareja inicial

De acuerdo con el plan de trabajo Seidy–Tiffany, el primer ciclo TDD de HU-10
inicia con **Seidy como Driver** y **Tiffany como Navigator**. Los roles se
rotarán en el siguiente incremento; este registro expresa la asignación
planificada de inicio y no atribuye evidencia de implementación todavía.

### Velocidad planificada

- Velocidad planificada de referencia: **36 SP por iteración**, según
  `plan-xp.md`.
- Alcance seleccionado para esta iteración: **31 SP**.
- Velocidad observada: **no disponible al inicio y no registrada**. Se calculará
  al cierre únicamente con historias que cumplan la Definition of Done.

### Condiciones de trabajo XP

- Cada historia comenzará con pruebas que expresen sus criterios de aceptación,
  seguirá con la implementación mínima y cerrará con refactorización.
- La pareja integrará incrementos pequeños y mantendrá propiedad colectiva del
  código.
- Ninguna historia de esta iteración se declara terminada en este inicio.

### HU-10 y HU-11 — Crear y publicar licitación

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-10 — Crear licitación | 5 | Terminada. |
| HU-11 — Publicar licitación | 3 | Terminada. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| Pruebas rojo HU-10 | Seidy | Tiffany | `8bb724b` |
| Implementación HU-10 | Tiffany | Seidy | `49ff9e7` |
| Pruebas cobertura HU-10 | Seidy | Tiffany | `8863d29` |
| Corrección y estilo HU-10 | Seidy | Tiffany | `6087703`, `dbab284` |
| Pruebas rojo HU-11 | Tiffany | Seidy | `dcd7ba0` |
| Implementación HU-11 | Seidy | Tiffany | `b6ed6a6` |
| Corrección formato HU-11 | Seidy | Tiffany | `60b84c5` |

#### Evidencia TDD rojo–verde–refactor

| Historia | Rojo | Verde | Refactorización |
| --- | --- | --- | --- |
| HU-10 | `8bb724b` agregó pruebas unitarias (presupuesto no positivo, estado Borrador) y de integración (CHECK, unicidad, persistencia). | `49ff9e7` implementó `Licitacion.Crear`, `CrearLicitacionService`, `ILicitacionRepository`, `LicitacionRepository`, API y MVC. | `6087703` corrigió la excepción duplicada en la prueba; `dbab284` ordenó imports para CI. No se justificó un refactor de código adicional. |
| HU-11 | `dcd7ba0` agregó pruebas unitarias (publicar desde Borrador, rechazo desde otros estados, rechazo con fecha vencida) y de integración (persistencia de estado y transición). | `b6ed6a6` implementó `Licitacion.Publicar(IClock)`, `LicitacionTransicion`, la migración `ImplementPublishTenderHu11` y la configuración EF Core. | `60b84c5` corrigió formato. No se identificó refactor justificado: la lógica de dominio es limpia, sin duplicación ni responsabilidades fusionadas. |

#### Resultado de pruebas (cierre de HU-10 + HU-11)

Ejecución local del 19 de agosto de 2026 con `dotnet test Licitaciones.sln`:

- 49 unitarias superadas.
- 59 de integración superadas contra PostgreSQL real.
- 3 funcionales superadas.
- 0 fallidas y 0 omitidas; 111 ejecutadas.

### HU-12 — Editar y cerrar licitación (refactor)

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-12 — Editar y cerrar licitación | 5 | Refactor completado. Dominio, servicio y pruebas unitarias existentes; sin endpoints HTTP ni registro DI todavía. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| Refactor HU-12 | Seidy | — | Sin commits (solo refactor local). |

#### Evidencia TDD rojo–verde–refactor

| Historia | Rojo | Verde | Refactorización |
| --- | --- | --- | --- |
| HU-12 | Las pruebas unitarias (6 en `EditarLicitacionServiceTests`, 4 en `EstadoEfectivoLicitacionTests`) existían desde la fase verde previa. | Todas las pruebas pasaban antes del refactor (59 unitarias, 3 funcionales). | `docs/bitacora-xp.md` registra este refactor. |

#### Refactorizaciones aplicadas

1. **Namespace `Editar/` alineado con convención `Crear/`** — Los tres archivos en
   `src/Licitaciones.Application/Licitaciones/Editar/` (`EditarLicitacionService`,
   `EditarLicitacionRequest`, `LicitacionNoEncontradaException`) cambiaron de
   `namespace Licitaciones.Application.Licitaciones` a
   `Licitaciones.Application.Licitaciones.Editar`, consistente con la convención
   establecida por la carpeta `Crear/`.

2. **Mapeo `LicitacionDto` centralizado** — Ambos servicios (`CrearLicitacionService`
   y `EditarLicitacionService`) construían `LicitacionDto` con las mismas 9
   propiedades. Se extrajo `LicitacionDto.FromEntity(Licitacion)` para eliminar la
   duplicación.

3. **`RepositorioEnMemoria` compartido en tests** — Dos implementaciones privadas
   separadas (`CrearLicitacionServiceTests` y `EditarLicitacionServiceTests`) se
   unificaron en `tests/Common/RepositorioEnMemoria.cs` con constructor
   configurable y propiedades `CodigoNormalizadoExiste`, `CodigoConsultado`,
   `LicitacionAgregada` y `MontoMinimoOferta`.

4. **`EstablecerEstado` extraído a helper compartido** — La función de reflexión
   para establecer el estado de una licitación en pruebas (duplicada en
   `EditarLicitacionServiceTests`, `EstadoEfectivoLicitacionTests` y
   `PublicarLicitacionTests`) se centralizó en
   `tests/Common/LicitacionTestHelper.cs` con `using static` en cada archivo de
   prueba.

#### Resultado de pruebas (refactor HU-12)

Ejecución local del 19 de agosto de 2026 con `dotnet test Licitaciones.sln`:

- 59 unitarias superadas (10 de HU-12: 6 editar + 4 estado efectivo).
- 3 funcionales superadas.
- Pruebas de integración no ejecutadas localmente (requieren Docker).
- 0 fallidas y 0 omitidas en las pruebas ejecutadas.

### HU-13 — Listar y consultar licitaciones

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-13 — Listar y consultar licitaciones | 3 | Terminada. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| Pruebas rojo HU-13 | Tiffany | Seidy | `b869316` |
| Implementación HU-13 | Seidy | Tiffany | `e62dca2` |
| Pruebas completas HU-13 | Seidy | Tiffany | `ce3445c` |
| Refactor HU-13 | Tiffany | Seidy | `d5761ec` |
| Refactor HU-13 (tests) | — | — | Sin commit (solo refactor local). |

#### Evidencia TDD rojo–verde–refactor

| Historia | Rojo | Verde | Refactorización |
| --- | --- | --- | --- |
| HU-13 | `b869316` agregó pruebas unitarias (listar con filtro de estado, cierre funcional, detalle con y sin ofertas, clasificación de nivel de aprobación) y de integración (persistencia de listado, filtro por estado, monto de ofertas). | `e62dca2` implementó `ConsultarLicitacionService`, `ILicitacionConsultaRepository`, `LicitacionConsultaRepository`, endpoints `GET` en `LicitacionesController` y registro DI. `ce3445c` completó pruebas de persistencia HTTP (listar, detalle, inexistente). | `d5761ec` simplificó la implementación. Refactor de tests extrajo `FixedClock` y `PublicarLicitacion` duplicados a `IntegrationTests/Common/LicitacionTestHelper.cs` compartido. |

#### Refactorizaciones aplicadas

1. **`FixedClock` y `PublicarLicitacion` extraídos a helper compartido** — Ambos
   archivos de prueba de integración de HU-13 (`ConsultarLicitacionHttpTests` y
   `ConsultarLicitacionPersistenceTests`) definían clases privadas idénticas
   `FixedClock : IClock` y métodos `PublicarLicitacion`. Se centralizaron en
   `tests/IntegrationTests/Common/LicitacionTestHelper.cs` con `using static`,
   eliminando la duplicación.

#### Resultado de pruebas (HU-13)

Ejecución local del 19 de agosto de 2026 con `dotnet test`:

- 68 unitarias superadas (9 de HU-13: 4 listar + 5 detalle).
- Pruebas de integración no ejecutadas localmente (requieren Docker).
- 0 fallidas y 0 omitidas en las pruebas ejecutadas.

### HU-14 — Registrar oferta

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-14 — Registrar oferta | 5 | Terminada: servicio de aplicación, persistencia PostgreSQL y endpoint REST implementados. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-14 | Seidy | Tiffany | `7b1fcdd` |
| VERDE HU-14 | Tiffany | Seidy | `3f24614` |
| Refactor HU-14 | Seidy | Tiffany | `1b59ae4` |

Los roles se registran a partir de la autoría alternada de los commits; Git no
conserva evidencia independiente del rol Navigator.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `7b1fcdd` — `test(ofertas): cubrir criterios de registrar oferta (HU-14)` | Agregó pruebas unitarias del servicio, pruebas HTTP y pruebas PostgreSQL para estado, vencimiento, duplicidad, presupuesto, monto positivo, FKs, CHECK e índice único. |
| VERDE | `3f24614` — `feat(ofertas): implementar registrar oferta (HU-14)` | Incorporó contrato y controlador API, servicio, repositorio, DTO y registro DI con el comportamiento mínimo para satisfacer las pruebas. |
| Refactor | `1b59ae4` — `refactor(ofertas): simplificar implementacion de HU-14` | Introdujo `OfertaDuplicadaException`, eliminó la clasificación de errores por texto, limitó la traducción de PostgreSQL al índice único esperado y centralizó `OfertaDto.FromEntity`. No agregó reglas ni endpoints. |

#### Commits

- `7b1fcdd` — pruebas de criterios de aceptación.
- `3f24614` — implementación del registro de ofertas.
- `1b59ae4` — refactorización sin cambio funcional.

#### Resultado

La línea base previa al refactor y la verificación final del 19 de agosto de
2026 se ejecutaron con `dotnet test Licitaciones.sln --no-restore` y PostgreSQL
real mediante Testcontainers. En ambas ejecuciones se obtuvieron:

- 76 pruebas unitarias superadas.
- 79 pruebas de integración superadas.
- 3 pruebas funcionales superadas.
- 0 fallidas y 0 omitidas; 158 ejecutadas.

El incremento permite registrar por API una oferta válida para una licitación
publicada y vigente. Rechaza con 409 la duplicidad y con 400 los demás errores
controlados cubiertos por HU-14. No incorpora listado, vistas MVC, auditoría de
rechazos ni clasificación de ofertas, que pertenecen a historias posteriores.

### HU-15 — Rechazar y auditar ofertas inválidas

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-15 — Rechazar y auditar ofertas inválidas | 3 | Terminada: códigos y mensajes específicos e inmutabilidad de ofertas registradas implementados y verificados. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-15 | Tiffany | Seidy | `cecc41a` |
| VERDE HU-15 | Seidy | Tiffany | `4319b06` |
| Refactor HU-15 | Tiffany | Seidy | `1ab4d8c` |

Los roles se registran a partir de la autoría alternada y el trabajo coordinado
de la pareja; Git conserva la autoría del Driver, no el rol Navigator.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `cecc41a` — `test(ofertas): cubrir criterios de rechazar y auditar ofertas inválidas (HU-15)` | Agregó cinco pruebas HTTP integradas para duplicidad, exceso de presupuesto, vencimiento y protección ante edición/eliminación, con PostgreSQL real y comprobación de evidencia inalterada. El resultado fue 1 aprobada y 4 fallidas por códigos/endpoints pendientes. |
| VERDE | `4319b06` — `fix(ofertas): implementar rechazar y auditar ofertas inválidas (HU-15)` | Incorporó códigos de error no procesable, traducción a `422`, servicio y repositorio de protección, rutas `PUT`/`DELETE` y registro DI mínimo para satisfacer las pruebas. |
| Refactor | `1ab4d8c` — `refactor(ofertas): simplificar protección de ofertas (HU-15)` | Aclaró los nombres de las dependencias del controlador y eliminó la construcción duplicada de `DomainException` sin cambiar reglas ni respuestas. |

#### Resultado

La línea base y la verificación final del 20 de agosto de 2026 se ejecutaron
con `dotnet test Licitaciones.sln --configuration Release --no-restore` contra
un esquema PostgreSQL limpio. El resultado final fue:

- 76 pruebas unitarias superadas.
- 84 pruebas de integración superadas.
- 3 pruebas funcionales superadas.
- 0 fallidas y 0 omitidas; 163 ejecutadas.

HU-15 devuelve `409` para duplicidad y `422` para vencimiento o exceso de
presupuesto. Los intentos de editar o eliminar ofertas registradas se rechazan
con `422`; en licitaciones cerradas se comunica la inmutabilidad y se conserva
la fila original como evidencia. No se incorporó una tabla o bitácora separada
para intentos rechazados, ni listado, clasificación o vistas futuras.

### HU-16 — Calcular mejor oferta y clasificación de ahorro

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-16 — Calcular mejor oferta y clasificación de ahorro | 5 | Terminada: selección, desempate, porcentaje, clasificación y respuesta sin ofertas implementados y verificados. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-16 | Seidy | Tiffany | `0220ec8` |
| VERDE HU-16 | Tiffany | Seidy | `eb50f93` |
| Refactor HU-16 | Seidy | Tiffany | `c54514f` |

Los roles se registran a partir de la autoría alternada de los commits; Git
conserva la autoría del Driver, no evidencia independiente del rol Navigator.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `0220ec8` — `test(ofertas): cubrir criterios de calcular mejor oferta y clasificación de ahorro (HU-16)` | Agregó cinco pruebas de Application y cinco HTTP para menor monto, desempate por `FechaRegistro`, ausencia de ofertas y los tres rangos de clasificación. Las pruebas Application confirmaron 5 fallos funcionales; la primera ejecución HTTP quedó bloqueada hasta iniciar Docker/Testcontainers. |
| VERDE | `eb50f93` — `feat(ofertas): implementar calcular mejor oferta y clasificación de ahorro (HU-16)` | Incorporó `CalculadoraMejorOferta`, `ResultadoMejorOferta`, consulta de ofertas desde infraestructura y amplió el DTO de detalle con identificador, monto, porcentaje, clasificación y mensaje sin ofertas. Las reglas permanecen fuera del controlador. |
| Refactor | `c54514f` — `refactor(ofertas): simplificar implementacion de HU-16` | Convirtió la calculadora pura y sin estado en estática, eliminó su instancia innecesaria en Application y reemplazó aserciones basadas en búsquedas dentro de JSON por validaciones directas de DTO y propiedades JSON. No agregó comportamiento. |

#### Commits

- `0220ec8` — pruebas de criterios de aceptación (ROJO).
- `eb50f93` — implementación mínima (VERDE).
- `c54514f` — refactorización sin cambio funcional.

No se crearon commits adicionales durante la actualización documental.

#### Resultado

La línea base y la verificación final del 20 de agosto de 2026 se ejecutaron
con `dotnet test Licitaciones.sln --no-restore` y PostgreSQL real mediante
Testcontainers. El resultado final fue:

- 81 pruebas unitarias superadas.
- 89 pruebas de integración superadas.
- 3 pruebas funcionales superadas.
- 0 fallidas y 0 omitidas; 173 ejecutadas.

El detalle de una licitación selecciona la oferta de menor monto y desempata
por la fecha de registro más temprana. Expone el porcentaje de ahorro y las
clasificaciones `Oferta conveniente`, `Oferta aceptable` u `Oferta válida sin
ahorro`; cuando no existen ofertas muestra `Sin ofertas válidas`. No se agregó
listado de ofertas, conversión monetaria, vistas MVC ni comportamiento de
historias futuras.

### HU-17 — Listar y consultar ofertas

#### Estado

| Historia | SP | Estado |
| --- | ---: | --- |
| HU-17 — Listar y consultar ofertas | 2 | Terminada: listado por licitación y detalle por identificador con proveedor, monto CRC/USD, fecha de registro e indicador de mejor oferta. |

#### Programación en pareja

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| ROJO HU-17 | Tiffany | Seidy | `fbfa912` |
| VERDE HU-17 | Seidy | Tiffany | `fc87fe0`, `7b49708` |
| Refactor HU-17 | Tiffany | Seidy | `6010638` |

Los roles se registran a partir de la autoría alternada de los commits; Git
conserva la autoría del Driver, no evidencia independiente del rol Navigator.

#### Evidencia TDD rojo–verde–refactor

| Fase | Commit | Resultado |
| --- | --- | --- |
| ROJO | `fbfa912` — `test(ofertas): cubrir criterios de listar y consultar ofertas (HU-17)` | Agregó dos pruebas HTTP con PostgreSQL real para listado por licitación y detalle en USD. Fijó proveedor, monto, moneda, fecha de registro e indicador de mejor oferta; ambas fallaron inicialmente con `405 Method Not Allowed`. |
| VERDE | `fc87fe0` — `feat(ofertas): implementar listar y consultar ofertas (HU-17)` | Incorporó servicio y DTO de consulta, repositorio con proveedor y tipo de cambio activo, endpoints GET y registro DI. `7b49708` ordenó imports para satisfacer CI sin cambiar comportamiento. |
| Refactor | `6010638` — `refactor(ofertas): simplificar implementacion de HU-17` | Extrajo la proyección compartida de oferta y proveedor en el repositorio. Además consolidó las 22 clases integradas en una colección xUnit que reutiliza un solo PostgreSQL Testcontainer, evitando la creación paralela de contenedores. No agregó comportamiento de negocio. |

#### Commits

- `fbfa912` — pruebas de criterios de aceptación (ROJO).
- `fc87fe0` — implementación mínima (VERDE).
- `7b49708` — corrección de formato/imports para CI.
- `6010638` — refactorización sin cambio funcional y fixture PostgreSQL compartida.

#### Resultado

La verificación final del 21 de agosto de 2026 se ejecutó con
`dotnet test Licitaciones.sln --configuration Release --no-restore --no-build`
y PostgreSQL real mediante una única instancia de Testcontainers compartida. El
resultado fue:

- 81 pruebas unitarias superadas.
- 91 pruebas de integración superadas.
- 3 pruebas funcionales superadas.
- 0 fallidas y 0 omitidas; 175 ejecutadas.

GitHub Actions confirmó el commit de refactor `6010638` con resultado exitoso
en la ejecución `32448992343`. La API lista ofertas por licitación y consulta
una oferta por identificador; presenta CRC o USD usando el tipo de cambio activo
y conserva CRC como valor persistido. La administración del tipo de cambio, su
fecha en la presentación y las vistas MVC permanecen fuera de HU-17.

## Cierre de la Iteración 2

Cierre documental registrado el 20 de agosto de 2026 sobre el commit `9966565`
de `main`, sin modificar código. Las fusiones se verificaron con
`git log main --merges`; el estado de GitHub Actions se consultó en la API
pública de GitHub durante este cierre; los componentes expuestos por HTTP se
verificaron contra los controladores y el registro DI reales de `main`.

### Verificación de fusión en `main`

Las ocho historias seleccionadas están fusionadas en `main` mediante los PR #19
a #26, cada uno con CI verde en el commit de fusión:

| PR | Commit de fusión | Historia | SP | Ejecución de CI en `main` | Resultado |
| --- | --- | --- | ---: | --- | --- |
| #19 | `cccfa2d` | HU-10 — Crear licitación | 5 | `32217608694` | success |
| #20 | `0fc34be` | HU-11 — Publicar licitación | 3 | `32258741686` | success |
| #21 | `cbd8fed` | HU-12 — Editar y cerrar licitación | 5 | `32285499762` | success |
| #22 | `1f5c453` | HU-13 — Listar y consultar licitaciones | 3 | `32318996472` | success |
| #23 | `d154284` | HU-14 — Registrar oferta | 5 | `32337758472` | success |
| #24 | `370e1ac` | HU-15 — Rechazar y auditar ofertas inválidas | 3 | `32383569802` | success |
| #25 | `0be6570` | HU-16 — Calcular mejor oferta y clasificación de ahorro | 5 | `32397368491` | success |
| #26 | `9966565` | HU-17 — Listar y consultar ofertas | 2 | `32450135648` | success |

### Definition of Done por historia

La evaluación aplica los criterios verificables de `plan-xp.md`. El criterio de
recorridos HTTP reales obliga solo cuando la historia expone MVC o API; el
criterio de exclusión alcanza a las historias cuyos criterios quedan cubiertos
únicamente por pruebas directas que omiten una frontera técnica relevante.

| Historia | SP | Cumple DoD | Observación verificada |
| --- | ---: | --- | --- |
| HU-10 | 5 | Sí | API (`POST /api/v1/licitaciones`) y MVC con recorridos HTTP reales; unicidad normalizada y CHECK de presupuesto probadas hasta PostgreSQL. |
| HU-11 | 3 | Sí | Sin endpoint propio: sus criterios viven en Domain (`Licitacion.Publicar`) y persistencia (`licitacion_transiciones`) y se ejercitan dentro de los recorridos HTTP reales de HU-14, HU-16 y HU-17 mediante el helper compartido de pruebas. La exposición HTTP propia queda registrada como ajuste para la Iteración 3. |
| HU-12 | 5 | **No** | Dominio, `EditarLicitacionService` y 10 pruebas unitarias fusionados, pero sin endpoints HTTP ni registro DI en Api/Web: ningún usuario puede invocar edición ni cierre. Sus criterios solo son alcanzables por prueba directa del servicio, lo que omite una frontera técnica relevante según el DoD. No cuenta para la velocidad observada. |
| HU-13 | 3 | Sí | Endpoints `GET` de listado y detalle con estado efectivo y mejor oferta; recorridos HTTP y de persistencia reales. |
| HU-14 | 5 | Sí | `POST /api/v1/ofertas` con orden de validación completo, FKs, CHECK e índice único probados sobre PostgreSQL real. |
| HU-15 | 3 | Sí | Rechazos `409`/`422` e inmutabilidad de ofertas registradas verificados por HTTP real con comprobación posterior de la evidencia persistida. |
| HU-16 | 5 | Sí | Selección, desempate, porcentaje y clasificación expuestos en el detalle y probados en Application y por HTTP real. |
| HU-17 | 2 | Sí | Listado por licitación y detalle por identificador con CRC/USD e indicador de mejor oferta, probados por HTTP real. |

El build Release, `dotnet format --verify-no-changes` (paso del workflow) y la
suite completa sin errores ni omitidas están cubiertos por el CI verde de cada
fusión para las ocho historias. La documentación de módulos quedó alineada en
este cierre; `docs/modulos/ofertas.md` se corrigió para reflejar HU-17.

### Velocidad planificada frente a observada

- Velocidad planificada de referencia: **36 SP** (`plan-xp.md`).
- Alcance seleccionado en el Planning Game: **31 SP**.
- Alcance fusionado en `main`: **31 SP** (las ocho historias).
- Velocidad observada al cierre: **26 SP**, contando únicamente las siete
  historias que cumplen la Definition of Done (HU-10, HU-11, HU-13, HU-14,
  HU-15, HU-16 y HU-17).
- Desviación: **−10 SP** frente a la referencia planificada y **−5 SP** frente
  al alcance seleccionado. La causa registrable es el pendiente de exposición
  HTTP y DI de HU-12, no trabajo de dominio faltante: sus reglas están
  implementadas y probadas a nivel servicio.

### Ciclos TDD y refactorizaciones

Las ocho historias siguieron ciclos rojo–verde–refactor con commits separados.
El CI documenta los ciclos: los commits de pruebas en rojo fallaron
(`dcd7ba0` en HU-11, `36c963a` en HU-12, `b869316` en HU-13, `7b1fcdd` en
HU-14, `cecc41a` en HU-15, `0220ec8` en HU-16 y `fbfa912` en HU-17) y también
fallaron dos verdes intermedios con pruebas o formato pendientes (`b6ed6a6` en
HU-11 y `e62dca2` en HU-13); el commit verde final de cada rama quedó en
success. `fc87fe0` falló únicamente por orden de imports y fue corregido en
`7b49708` sin cambiar comportamiento.

Refactorizaciones relevantes del cierre de cada ciclo (detalle por historia en
las secciones anteriores):

- HU-12: namespaces `Editar/` alineados, `LicitacionDto.FromEntity`,
  `RepositorioEnMemoria` y helper `EstablecerEstado` compartidos.
- HU-13: helpers `FixedClock` y `PublicarLicitacion` extraídos a un shared
  helper de integración.
- HU-14: `OfertaDuplicadaException` sustituye la comparación textual de errores
  y la traducción de PostgreSQL se acota al índice único esperado.
- HU-15: nombres de dependencias aclarados y construcción duplicada de
  `DomainException` eliminada.
- HU-16: `CalculadoraMejorOferta` convertida en servicio estático puro y
  aserciones JSON sustituidas por validaciones directas del DTO.
- HU-17: proyección compartida de oferta/proveedor y consolidación de las 22
  clases de integración en una colección xUnit que reutiliza un único
  Testcontainer PostgreSQL.

### Participación Seidy/Tiffany

La iteración acumula 46 commits entre `979e223` y `9966565`: Seidy Oporta 26
(incluye 5 merges registrados como `Seidy06`) y Tiffany Alfaro 20 (incluye 4
merges registrados como `tiffanyyulieth08`). La autoría alternó en las fases
rojo, verde, refactor y documental de cada historia, y las tablas por historia
registran la rotación Driver/Navigator. Git conserva la autoría del Driver; el
rol Navigator se reconstruye a partir del trabajo coordinado de la pareja, sin
atribuir sesiones sin evidencia.

### Integración continua

El workflow ejecuta restore, `dotnet format --verify-no-changes`, build Release
y la suite completa con PostgreSQL 16 como servicio, en push y pull_request
hacia `main`. Los ocho commits de fusión de la iteración terminaron en success
(ejecuciones listadas en la tabla de fusiones). El workflow sigue sin medir
cobertura, análisis estático ni construcción Docker.

### Resultado de la demostración

No existe en el repositorio un acta de demostración ni una aprobación firmada
del cliente, por lo que no se registra retroalimentación externa. Lo demostrable
y reproducible del incremento es:

- Por HTTP real: crear licitación, registrar una oferta con sus reglas,
  comprobar rechazos `409`/`422` y la inmutabilidad de ofertas, listar y
  consultar licitaciones con estado efectivo, mejor oferta y clasificación de
  ahorro, y listar/consultar ofertas en CRC o USD.
- A nivel dominio y persistencia, ejercitado dentro de los recorridos HTTP
  anteriores: publicar (con transición registrada) y el cierre funcional por
  vencimiento.
- No ejecutable aún por un usuario final: publicar, editar y cerrar desde una
  superficie HTTP o MVC propia, porque HU-11/HU-12 no exponen endpoints ni
  vistas.

La suite que respalda esta demostración es la registrada en `pruebas.md`:
175 pruebas superadas, 0 fallidas y 0 omitidas.

### Retroalimentación y ajustes para la Iteración 3

Sin acta del cliente, los ajustes se derivan exclusivamente de brechas
verificables en el código fusionado:

1. Exponer publicación, edición y cierre mediante endpoints API con registro
   DI, completando la superficie de usuario de HU-11 y HU-12 y haciendo
   ejecutable de punta a punta el ciclo crear → publicar → ofertar → cerrar.
2. Incorporar paginación, filtro y ordenamiento a los listados de licitaciones
   y ofertas: los enunciados de HU-13 y HU-17 los mencionan y los `GET`
   actuales aún no aceptan esos parámetros.
3. Construir las vistas MVC de licitaciones y ofertas; hoy solo existe el
   formulario de creación.
4. Priorizar HU-18 y HU-19 en el Planning Game: el nivel de aprobación del
   detalle permanece `null` y la administración del tipo de cambio no existe;
   HU-17 solo consulta el activo sembrado.

### Pequeña liberación

La etiqueta `v0.2.0` no existe todavía (`git tag -l` muestra solo `v0.1.0`).
Se creará después de fusionar la rama de cierre documental, identificando el
incremento HU-10 a HU-17 con la salvedad explícita de que la exposición HTTP de
publicar/editar/cerrar queda pendiente para la Iteración 3.

---

## Regularización retrospectiva de trazabilidad — Iteración 1

Esta regularización se realizó después de que la Iteración 1 ya había sido
implementada, documentada, auditada y cerrada, y después de la creación de la
etiqueta `v0.1.0`. Se detectó que el catálogo vigente HU-00 a HU-09 no contaba
con una asociación completa a GitHub Issues.

Se crearon retrospectivamente los Issues #37 a #45 para HU-00 a HU-05 y HU-07
a HU-09. HU-06 no recibió un Issue duplicado: conserva el Issue histórico #8,
titulado “HU-01 Registrar proveedor”, cuya equivalencia con HU-06 ya estaba
documentada y fue aclarada mediante un comentario posterior. También se
añadieron comentarios de relación a los PR existentes, sin modificar sus
descripciones ni los commits que ya contenían referencias históricas.

| Historia vigente | Issue | PR principal | Rama principal |
| --- | --- | --- | --- |
| HU-00 — Inicializar repositorio | [#37](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/37) | [#2](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/2) | `chore/inicializacion` |
| HU-01 — Documentar plan XP e historias | [#38](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/38) | [#10](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/10) | `iteracion-1/hu-01-plan-xp` |
| HU-02 — Modelar entidades de dominio | [#39](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/39) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | `iteracion-1/hu-00-hu-05-base` |
| HU-03 — Configurar EF Core y PostgreSQL | [#40](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/40) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | `iteracion-1/hu-00-hu-05-base` |
| HU-04 — Migraciones y semillas | [#41](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/41) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | `iteracion-1/hu-00-hu-05-base` |
| HU-05 — Abstraer el reloj | [#42](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/42) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | `iteracion-1/hu-00-hu-05-base` |
| HU-06 — Registrar proveedor | [#8](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/8) (histórico HU-01) | [#12](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/12) | `iteracion-1/hu-06-registrar-proveedor` |
| HU-07 — Editar proveedor | [#43](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/43) | [#14](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/14) | `iteracion-1/hu-07-editar-proveedor` |
| HU-08 — Eliminar lógicamente proveedor | [#44](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/44) | [#15](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/15) | `iteracion-1/hu-08-eliminar-proveedor` |
| HU-09 — Listar y consultar proveedores | [#45](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/45) | [#13](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/13) | `iteracion-1/hu-09-consultar-proveedores` |

No se reescribieron commits, Issues históricos ni Pull Requests y no se repitió
el cierre de la iteración. El objeto anotado de `v0.1.0` y su commit objetivo
permanecen intactos.

## Iteración 1 — Base técnica y proveedores

### Objetivo

Establecer una solución .NET 9 por capas, persistencia reproducible en PostgreSQL, reloj abstraído e integración continua, y entregar la gestión completa de proveedores mediante MVC y API REST.

### Estado de las historias

La valoración corresponde al catálogo vigente en `historias-usuario.md`.

| Historia | SP | Estado y evidencia observable |
| --- | ---: | --- |
| HU-00 — Inicializar repositorio | 2 | Terminada: solución y carpetas `src`, `tests`, `docs`, `docker` y `k8s` existentes. Commits `234005c`, `1db3d2b`. |
| HU-01 — Documentar plan XP e historias | 2 | Terminada: plan, catálogo, visión y estructura documental. Commits `64f1aa5`, `70aeefc`, `f8c9079`, `7e9c8e2`, `8fa5928`. |
| HU-02 — Modelar entidades de dominio | 5 | Terminada como base de dominio: proveedor, licitación, oferta, nivel, tipo de cambio y estados; Domain no referencia EF Core ni ASP.NET. Commit `145e83e`. |
| HU-03 — Configurar EF Core y PostgreSQL | 5 | Terminada: contexto, Npgsql, mapeos y auditoría automática. Commits `5cf842f`, `145e83e`. |
| HU-04 — Migraciones y semillas | 3 | Terminada: tres migraciones, cinco estados, tres niveles y tipo de cambio USD/CRC. Commits `5cf842f`, `145e83e`, `cc43bd2`. |
| HU-05 — Abstraer el reloj | 2 | Terminada: `IClock`, `SystemClock` y `FixedClock`; usada por auditoría y baja lógica. Commit `145e83e`. |
| HU-06 — Registrar proveedor | 3 | Terminada en MVC y API con normalización Unicode y conflicto concurrente controlado. Commits `1666a8d`, `23aa497`, `276d9af`, `9903e00`. |
| HU-07 — Editar proveedor | 2 | Terminada en MVC y API con unicidad y concurrencia mediante `xmin`. Commits `18dd3fc`, `dac1452`. |
| HU-08 — Dar de baja proveedor | 3 | Terminada: confirmación MVC, DELETE en API, `DeletedAt`, filtro global e histórico explícito desde MVC y API. Las pruebas conservan filas y ofertas relacionadas. Commits `a74b9cd`, `cc43bd2`, `aed1feb`, `38efa9f` y rama de cierre. |
| HU-09 — Listar y consultar proveedores | 3 | Terminada: detalle, paginación, filtro y ordenamiento en MVC y API. Commits `01f2499`, `334e618`, `6631011`. |
| **Total observado** | **30** | **HU-00 a HU-09 cuentan con evidencia ejecutable o documental en el alcance definido.** |

Las entidades distintas de proveedores son una base de HU-02 a HU-05; no implican que los casos de uso de iteraciones posteriores estén implementados.

### Programación en pareja y rotaciones

Git registra autoría, pero no guarda el rol Navigator. La tabla reconstruye las rotaciones a partir de los commits alternados: quien figura como autor del incremento se registra como Driver y la otra integrante como Navigator. No se atribuyen sesiones que no tengan esa evidencia.

| Sesión o incremento | Driver | Navigator | Evidencia |
| --- | --- | --- | --- |
| Pruebas iniciales de registro | Seidy | Tiffany | `ad3f311`, `be3c836` |
| Implementación inicial de registro | Tiffany | Seidy | `f274b20`, `f597141` |
| Pruebas de Unicode y concurrencia | Seidy | Tiffany | `1666a8d` |
| Corrección verde de HU-06 | Tiffany | Seidy | `23aa497` |
| Refactorización de HU-06 | Seidy | Tiffany | `276d9af` |
| Pruebas de HU-09 | Tiffany | Seidy | `01f2499` |
| Implementación de HU-09 | Seidy | Tiffany | `334e618` |
| Pruebas de HU-07 | Seidy | Tiffany | `18dd3fc` |
| Implementación de HU-07 | Tiffany | Seidy | `dac1452` |
| Pruebas de HU-08 | Tiffany | Seidy | `a74b9cd` |
| Persistencia y caso de uso HU-08 | Seidy | Tiffany | `cc43bd2`, `aed1feb` |
| Pruebas finales del CRUD API | Seidy | Tiffany | `ece009f` |

### Evidencia TDD rojo–verde–refactor

| Historia | Rojo | Verde | Refactorización o consolidación |
| --- | --- | --- | --- |
| Registro histórico, luego HU-06 | `ad3f311`, `be3c836`, `0fd4129` agregaron pruebas antes del comportamiento completo. | `f274b20`, `f597141`, `89f0768`, `1516d2c` incorporaron dominio, servicio, API y MVC. | `1b1aed4` simplificó registro y normalización. |
| HU-06 | `1666a8d` reprodujo equivalencia Unicode y carrera concurrente. | `23aa497` normalizó Unicode y tradujo la violación única a 409. | `276d9af` unificó el cálculo de nombre legible/comparable y centralizó el nombre del índice. |
| HU-09 | `01f2499` fijó contratos de consulta, paginación, filtro, API y MVC. | `334e618` agregó servicio, repositorio, endpoints y vistas. | La proyección a DTO y ViewModel dejó las entidades EF fuera de las interfaces; no hay un commit `refactor` separado. |
| HU-07 | `18dd3fc` fijó edición, duplicidad y versión desactualizada. | `dac1452` agregó edición con `xmin` y contratos MVC/API. | Reutilizó el normalizador y validador existentes; no hay un commit `refactor` separado. |
| HU-08 | `a74b9cd` fijó reloj, filtro, conservación histórica, 204/404 y confirmación MVC. | `cc43bd2` y `aed1feb` agregaron persistencia y baja lógica. | El filtro global concentró la exclusión de bajas; no hay un commit `refactor` separado. |

### Refactorizaciones relevantes

- Centralización de normalización y validación en Domain.
- Controladores MVC y API delegan en servicios de Application.
- DTO y ViewModel separan interfaces públicas de entidades persistidas.
- Nombre del índice único compartido para traducir solo el conflicto esperado.
- `IClock` evita tiempo global en auditoría y baja lógica.
- Filtro global evita repetir `DeletedAt == null` en cada consulta activa.

### Integración continua

`69316c7` agregó `.github/workflows/ci.yml` y `4cd33eb` incorporó PostgreSQL para integración. El workflow se activa en push y pull request hacia `main`, usa Ubuntu, .NET 9 y PostgreSQL 16, y ejecuta restore, build Release y test. Los incrementos se integraron mediante los PR #9 y #11 a #16. El workflow actual no incluye cobertura, análisis estático ni construcción Docker.

### Resultado de pruebas

Verificación local reproducida el 15 de agosto de 2026 con `dotnet test Licitaciones.sln --configuration Release`:

- 36 unitarias superadas.
- 51 de integración superadas contra PostgreSQL real.
- 1 funcional HTTP superada.
- 0 fallidas y 0 omitidas; 88 ejecutadas.

### Retroalimentación incorporada

- La auditoría de la numeración anterior produjo la equivalencia histórica entre HU-01/HU-02/HU-03/HU-04 de proveedores y HU-06/HU-09/HU-07/HU-08 del catálogo vigente; no se reescribió el historial.
- La revisión técnica del registro pidió equivalencia Unicode y manejo de inserciones concurrentes; se incorporó en HU-06 con pruebas y respuestas 409.
- La revisión del cierre pidió cubrir el CRUD REST completo; `ece009f` añadió esa evidencia.
- La auditoría final detectó que las pruebas anteriores invocaban controladores directamente. La rama de cierre añadió `WebApplicationFactory`, descubrió y corrigió la activación ambigua de controladores, y cubrió API y MVC mediante HTTP real.
- El histórico de HU-08 quedó disponible mediante rutas explícitas en MVC y API sin alterar las consultas activas.

No hay en el repositorio un acta o comentario atribuible al cliente con retroalimentación adicional; por eso no se inventa una aceptación externa.

### Velocidad

- Velocidad planificada inicial: **36 SP**, registrada en `plan-xp.md`.
- Alcance seleccionado real HU-00 a HU-09: **30 SP** según el catálogo vigente.
- Velocidad observada: **30 SP**, porque las diez historias tienen evidencia terminada.
- Diferencia frente a la previsión inicial: **−6 SP**. La diferencia proviene de que la suma vigente del alcance seleccionado es 30, no 36; no se agregan historias futuras para completar la cifra.

### Resultado de la demostración

La demostración técnica reproducible del incremento permite registrar, listar, filtrar, ordenar, consultar, editar con control de versión, dar de baja y consultar el histórico desde MVC y API. La baja conserva la fila y sus ofertas, y la excluye de consultas activas. El recorrido está respaldado por 88 pruebas automatizadas superadas, incluidas pruebas HTTP end-to-end del CRUD y una prueba funcional de la plantilla MVC.

No existe en el repositorio un acta de demostración presencial ni una aprobación firmada del cliente. El resultado documentado es, por tanto, la demostración técnica verificable del incremento y no una aceptación externa inferida.

### Pequeña liberación

La Iteración 1 se libera como `v0.1.0` después de superar restore, verificación
de formato, build Release y las 88 pruebas. La etiqueta identifica el incremento
HU-00 a HU-09 y no implica que las historias de iteraciones posteriores estén
terminadas.
