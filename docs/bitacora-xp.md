# Bitácora XP

## Iteración 2 — Ciclo de licitaciones y ofertas

**Estado: INICIADA — 18 de agosto de 2026.**

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

---

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
