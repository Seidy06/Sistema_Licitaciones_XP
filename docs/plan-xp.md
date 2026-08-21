# Plan de trabajo XP

## Propósito

Este plan adopta formalmente el catálogo aprobado de historias HU-00 a HU-37.
La selección y el orden podrán ajustarse mediante el Planning Game según la
retroalimentación del cliente, la velocidad observada y el aprendizaje del
equipo, sin perder la trazabilidad de las decisiones anteriores.

## Plan de iteraciones

El proyecto se organiza en cuatro iteraciones de duración uniforme. Cada
iteración concluye con una pequeña liberación verificable y alimenta la
planificación de la siguiente.

La velocidad planificada inicial es de **36 puntos de historia por iteración**.
Este valor se revisa al cierre de cada iteración con la velocidad realmente
observada, sin ampliar artificialmente el alcance de una iteración en curso.

| Iteración | Historias seleccionadas | Objetivo de la pequeña liberación |
| --- | --- | --- |
| Iteración 1 | HU-00 a HU-09 | Establecer la base técnica, el modelo persistente y la gestión de proveedores. |
| Iteración 2 | HU-10 a HU-17 | Completar la gestión de licitaciones y ofertas con sus reglas de negocio. |
| Iteración 3 | HU-18 a HU-27 | Incorporar aprobación, conversión monetaria, experiencia web y API REST documentada. |
| Iteración 4 | HU-28 a HU-37 | Consolidar pruebas, contenedores, despliegue, integración continua, documentación y etiquetado. |

### Iteración 1: HU-00 a HU-09

- Inicialización y planificación XP.
- Modelo de dominio, persistencia y abstracción del reloj.
- Registro, edición, eliminación lógica, listado y consulta de proveedores.
- TDD, refactorización e integración continua aplicados al incremento.

La asociación con GitHub Issues se regularizó retrospectivamente después del
cierre, la auditoría y la creación de `v0.1.0`. El Issue #8 se conserva con su
numeración histórica y corresponde a HU-06 del catálogo vigente:

| Historia vigente | Issue | PR principal | Evidencia complementaria |
| --- | --- | --- | --- |
| HU-00 — Inicializar repositorio | [#37](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/37) | [#2](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/2) | [PR #11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) |
| HU-01 — Documentar plan XP e historias | [#38](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/38) | [#10](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/10) | [PR #5](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/5) |
| HU-02 — Modelar entidades de dominio | [#39](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/39) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | — |
| HU-03 — Configurar EF Core y PostgreSQL | [#40](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/40) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | [PR #6](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/6) |
| HU-04 — Migraciones y semillas | [#41](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/41) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | — |
| HU-05 — Abstraer el reloj | [#42](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/42) | [#11](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/11) | — |
| HU-06 — Registrar proveedor | [#8](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/8) (numeración histórica HU-01) | [#12](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/12) | [PR #9](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/9), [PR #16](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/16) |
| HU-07 — Editar proveedor | [#43](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/43) | [#14](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/14) | [PR #16](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/16) |
| HU-08 — Eliminar lógicamente proveedor | [#44](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/44) | [#15](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/15) | [PR #16](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/16) |
| HU-09 — Listar y consultar proveedores | [#45](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/45) | [#13](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/13) | [PR #16](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/16) |

### Iteración 2: HU-10 a HU-17

- Creación, publicación, edición, cierre, listado y consulta de licitaciones.
- Registro, validación, auditoría, evaluación, listado y consulta de ofertas.
- TDD y refactorización de las reglas de negocio del incremento.

La asociación con GitHub Issues se regularizó retrospectivamente después del
cierre, la auditoría y la creación de `v0.2.0`. Esta tabla documenta la relación
actual sin atribuir los Issues a la ejecución histórica de la iteración:

| Historia | Issue retrospectivo | PR principal | Evidencia complementaria |
| --- | --- | --- | --- |
| HU-10 — Crear licitación | [#29](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/29) | [#19](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/19) | — |
| HU-11 — Publicar licitación | [#30](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/30) | [#20](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/20) | [PR #28](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/28) |
| HU-12 — Editar y cerrar licitación | [#31](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/31) | [#21](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/21) | [PR #28](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/28) |
| HU-13 — Listar y consultar licitaciones | [#32](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/32) | [#22](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/22) | [PR #28](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/28) |
| HU-14 — Registrar oferta | [#33](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/33) | [#23](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/23) | — |
| HU-15 — Rechazar y auditar ofertas inválidas | [#34](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/34) | [#24](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/24) | — |
| HU-16 — Calcular mejor oferta y clasificación de ahorro | [#35](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/35) | [#25](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/25) | — |
| HU-17 — Listar y consultar ofertas | [#36](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/36) | [#26](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/26) | [PR #28](https://github.com/Seidy06/Sistema_Licitaciones_XP/pull/28) |

### Iteración 3: HU-18 a HU-27

- Niveles de aprobación y conversión CRC/USD.
- Página inicial, navegación, presentación visual y operaciones web.
- API REST versionada y documentación OpenAPI/Swagger.
- Pequeñas liberaciones frecuentes e integración continua.

### Iteración 4: HU-28 a HU-37

- Pruebas unitarias, de integración y funcionales aplicando TDD.
- Docker, Docker Compose y Kubernetes.
- Integración continua, documentación técnica y etiquetado de la entrega.

## Trazabilidad de la auditoría de la Iteración 1

La auditoría de la Iteración 1 se realizó con una numeración anterior para las
historias de proveedores. A partir de esta actualización se utiliza la
siguiente equivalencia:

| Historia auditada anteriormente | Historia del catálogo actual |
| --- | --- |
| HU-01 — Registrar proveedor | HU-06 — Registrar proveedor |
| HU-02 — Consultar proveedores | HU-09 — Listar y consultar proveedores |
| HU-03 — Editar proveedor | HU-07 — Editar proveedor |
| HU-04 — Eliminar proveedor | HU-08 — Eliminar lógicamente proveedor |

Los commits históricos no se modificarán ni se reescribirán. Sus identificadores
y mensajes forman parte de la evidencia XP del repositorio y deben interpretarse
mediante esta tabla de equivalencias.

## Prácticas XP de seguimiento

- Planning Game para seleccionar y ajustar historias.
- TDD mediante ciclos rojo, verde y refactorización.
- Integración continua y pequeñas liberaciones.
- Programación en pareja con rotación de Driver y Navigator.
- Propiedad colectiva del código y estándares compartidos.
- Ritmo sostenible y retroalimentación frecuente del cliente.

## Definition of Done

La Definition of Done verificable para cada historia seleccionada exige:

- criterios de aceptación implementados y trazables a pruebas;
- reglas de negocio en Domain/Application, sin duplicación en controladores;
- persistencia protegida por las restricciones PostgreSQL aplicables;
- pruebas unitarias e integración proporcionales al cambio;
- recorridos HTTP reales cuando la historia expone MVC o API;
- build Release y suite completa sin errores ni pruebas omitidas;
- `dotnet format --verify-no-changes` correcto;
- GitHub Actions verde en el commit integrado;
- documentación y bitácora alineadas con la implementación;
- árbol de trabajo limpio y pequeña liberación etiquetada al cerrar la iteración.

Una historia no se registra como terminada si alguno de sus criterios permanece
disponible únicamente como intención documental o prueba directa que omita una
frontera técnica relevante.
