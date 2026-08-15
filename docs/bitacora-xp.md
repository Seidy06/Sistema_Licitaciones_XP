# Bitácora XP

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
| HU-08 — Dar de baja proveedor | 3 | Terminada: confirmación MVC, DELETE en API, `DeletedAt`, filtro global e histórico interno con `IgnoreQueryFilters()`. No existe una pantalla de reportes históricos. Commits `a74b9cd`, `cc43bd2`, `aed1feb`, `38efa9f`. |
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

- 35 unitarias superadas.
- 47 de integración superadas contra PostgreSQL real.
- 0 fallidas y 0 omitidas; 82 ejecutadas.
- El proyecto funcional compila, pero no contiene pruebas detectables.

### Retroalimentación incorporada

- La auditoría de la numeración anterior produjo la equivalencia histórica entre HU-01/HU-02/HU-03/HU-04 de proveedores y HU-06/HU-09/HU-07/HU-08 del catálogo vigente; no se reescribió el historial.
- La revisión técnica del registro pidió equivalencia Unicode y manejo de inserciones concurrentes; se incorporó en HU-06 con pruebas y respuestas 409.
- La revisión del cierre pidió cubrir el CRUD REST completo; `ece009f` añadió esa evidencia.

No hay en el repositorio un acta o comentario atribuible al cliente con retroalimentación adicional; por eso no se inventa una aceptación externa.

### Velocidad

- Velocidad planificada inicial: **36 SP**, registrada en `plan-xp.md`.
- Alcance seleccionado real HU-00 a HU-09: **30 SP** según el catálogo vigente.
- Velocidad observada: **30 SP**, porque las diez historias tienen evidencia terminada.
- Diferencia frente a la previsión inicial: **−6 SP**. La diferencia proviene de que la suma vigente del alcance seleccionado es 30, no 36; no se agregan historias futuras para completar la cifra.

### Resultado de la demostración

La demostración técnica reproducible del incremento permite registrar, listar, filtrar, ordenar, consultar, editar con control de versión y dar de baja proveedores desde MVC y API. La baja conserva la fila y la excluye de consultas activas. El recorrido está respaldado por 82 pruebas automatizadas superadas, incluidas pruebas HTTP del CRUD.

No existe en el repositorio un acta de demostración presencial ni una aprobación firmada del cliente. El resultado documentado es, por tanto, la demostración técnica verificable del incremento y no una aceptación externa inferida.
