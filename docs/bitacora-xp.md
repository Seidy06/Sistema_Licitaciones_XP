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
| 3 | HU-14 — Registrar oferta | Alta | 5 | HU-11, HU-06 y HU-05: requiere licitación publicada, proveedor y reloj inyectable. | Seleccionada; no terminada |
| 4 | HU-12 — Editar y cerrar licitación | Alta | 5 | HU-10 y HU-14 para comprobar las restricciones de edición frente a ofertas existentes. | Refactor completado; sin endpoints HTTP ni DI. |
| 5 | HU-15 — Rechazar y auditar ofertas inválidas | Alta | 3 | HU-14 y HU-12 para verificar duplicidad, exceso de presupuesto, vencimiento e inmutabilidad tras el cierre. | Seleccionada; no terminada |
| 6 | HU-16 — Calcular mejor oferta y clasificación de ahorro | Alta | 5 | HU-14 para disponer de ofertas válidas. El nivel de aprobación correspondiente depende de HU-18, planificada para la Iteración 3. | Seleccionada; no terminada |
| 7 | HU-13 — Listar y consultar licitaciones | Media | 3 | HU-12 y HU-16 para mostrar estado efectivo y mejor oferta. El nivel de aprobación depende de HU-18. | Terminada. |
| 8 | HU-17 — Listar y consultar ofertas | Media | 2 | HU-14 y HU-16. La presentación alternable en USD depende del servicio de conversión de HU-19, planificado para la Iteración 3. | Seleccionada; no terminada |
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
