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

La Iteración 3 inicia con **38 SP seleccionados** y mantiene **36 SP como
velocidad planificada de referencia**. La diferencia de 2 SP se registra como
riesgo de planificación; no se declara velocidad observada hasta el cierre.
El orden conserva HU-18 a HU-27: primero capacidades de negocio, después
experiencia web y finalmente consolidación de API y Swagger. Las dependencias
pueden hacer que una pareja adelante preparación técnica sin marcar historias
como terminadas.

| Orden | Historia | Prioridad | SP | Dependencias principales | Driver | Navigator | Issue | Rama prevista |
| ---: | --- | --- | ---: | --- | --- | --- | --- | --- |
| 1 | HU-18 — Niveles de aprobación | Alta | 5 | Modelo persistente de HU-04; consumo posterior por HU-23/HU-26. | Tiffany | Seidy | [#47](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/47) | `iteracion-3/hu-18-niveles-aprobacion` |
| 2 | HU-19 — Tipo de cambio y conversión | Alta | 5 | Tipo de cambio sembrado en HU-04 y montos de HU-17; habilita presentación web. | Seidy | Tiffany | [#48](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/48) | `iteracion-3/hu-19-tipo-cambio` |
| 3 | HU-20 — Landing page | Media | 3 | Contenido del flujo HU-10 a HU-19. | Tiffany | Seidy | [#49](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/49) | `iteracion-3/hu-20-landing-page` |
| 4 | HU-21 — Navegación global | Media | 2 | Rutas MVC y enlace a Swagger de HU-27. | Seidy | Tiffany | [#50](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/50) | `iteracion-3/hu-21-navegacion-global` |
| 5 | HU-22 — Tema claro/oscuro | Baja | 2 | Layout compartido de HU-21. | Tiffany | Seidy | [#51](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/51) | `iteracion-3/hu-22-tema-claro-oscuro` |
| 6 | HU-23 — CRUD web completo | Alta | 8 | Casos de uso HU-06 a HU-19; niveles y tipo de cambio de HU-18/HU-19. | Seidy | Tiffany | [#52](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/52) | `iteracion-3/hu-23-crud-web-completo` |
| 7 | HU-24 — Mensajería | Media | 2 | Operaciones MVC de HU-23. | Tiffany | Seidy | [#53](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/53) | `iteracion-3/hu-24-mensajeria` |
| 8 | HU-25 — Formato es-CR | Baja | 1 | Montos y conversión de HU-19; vistas de HU-23. | Seidy | Tiffany | [#54](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/54) | `iteracion-3/hu-25-formato-es-cr` |
| 9 | HU-26 — API REST versionada | Alta | 8 | Casos de uso de todos los módulos, incluidos HU-18/HU-19. | Tiffany | Seidy | [#55](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/55) | `iteracion-3/hu-26-api-rest-versionada` |
| 10 | HU-27 — OpenAPI/Swagger | Media | 2 | Contratos y endpoints consolidados en HU-26. | Seidy | Tiffany | [#56](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/56) | `iteracion-3/hu-27-swagger` |

### Iteración 4: HU-28 a HU-37

- Pruebas unitarias, de integración y funcionales aplicando TDD.
- Docker, Docker Compose y Kubernetes.
- Integración continua, documentación técnica y etiquetado de la entrega.

La Iteración 4 inicia el 24 de agosto de 2026 con **45 SP seleccionados** y
mantiene **36 SP como velocidad planificada de referencia**. La diferencia de
9 SP se registra como riesgo explícito de alcance: es el catálogo completo del
release final y no se ampliará con trabajo adicional. No existe todavía
velocidad observada de esta iteración y no se calculará hasta contar con
evidencia de cierre. Ninguna historia está marcada como terminada.

El orden conserva HU-28 a HU-37 y sus dependencias son: HU-28 consolida las
pruebas unitarias y su cobertura sobre el dominio ya construido; HU-29
formaliza la integración contra PostgreSQL real (infraestructura Testcontainers
existente desde la Iteración 2); HU-30 añade E2E de navegador sobre la
experiencia web completa de la Iteración 3; HU-31 introduce el `Dockerfile`
multi-stage; HU-32 orquesta el entorno local y depende de HU-31; HU-33 y
HU-34 trasladan aplicación y persistencia a Kubernetes (`/k8s`), en ese orden;
HU-35 integra todo en el pipeline de CI (pruebas, cobertura, formato,
imagen Docker, validación K8s y auditoría de dependencias); HU-36 cierra la
documentación técnica; HU-37 etiqueta la entrega evaluable. El desarrollo
aplicará ciclos TDD rojo–verde–refactor, programación en pareja, integración
continua y pequeñas liberaciones; las Issues se usan únicamente como
trazabilidad XP.

| Orden | Historia | Prioridad | SP | Dependencias principales | Driver | Navigator | Issue | Rama prevista |
| ---: | --- | --- | ---: | --- | --- | --- | --- | --- |
| 1 | HU-28 — Pruebas unitarias del dominio | Alta | 5 | Reglas de negocio de HU-06 a HU-19. | Tiffany | Seidy | [#69](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/69) | `iteracion-4/hu-28-pruebas-unitarias-dominio` |
| 2 | HU-29 — Integración PostgreSQL real | Alta | 5 | Fixture Testcontainers de la Iteración 2. | Seidy | Tiffany | [#70](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/70) | `iteracion-4/hu-29-integracion-postgresql` |
| 3 | HU-30 — Pruebas E2E de navegador | Alta | 8 | Experiencia web completa de la Iteración 3; HU-31/HU-32 para ejecución en CI. | Tiffany | Seidy | [#71](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/71) | `iteracion-4/hu-30-pruebas-e2e` |
| 4 | HU-31 — Dockerfile multi-stage | Alta | 3 | Aplicación .NET 9 consolidada. | Seidy | Tiffany | [#72](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/72) | `iteracion-4/hu-31-dockerfile` |
| 5 | HU-32 — Docker Compose local | Alta | 3 | HU-31. | Tiffany | Seidy | [#73](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/73) | `iteracion-4/hu-32-docker-compose` |
| 6 | HU-33 — Manifiestos K8s de la app | Alta | 5 | Imagen de HU-31. | Seidy | Tiffany | [#74](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/74) | `iteracion-4/hu-33-k8s-app` |
| 7 | HU-34 — Persistencia PostgreSQL en K8s | Alta | 5 | HU-33. | Tiffany | Seidy | [#75](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/75) | `iteracion-4/hu-34-k8s-postgresql` |
| 8 | HU-35 — Pipeline de CI completo | Alta | 5 | HU-28 a HU-34. | Seidy | Tiffany | [#76](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/76) | `iteracion-4/hu-35-pipeline-ci` |
| 9 | HU-36 — Documentación final en /docs | Alta | 5 | Resultados de HU-28 a HU-35. | Tiffany | Seidy | [#77](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/77) | `iteracion-4/hu-36-documentacion-final` |
| 10 | HU-37 — Etiquetado de entrega final | Alta | 1 | Todo lo anterior. | Seidy | Tiffany | [#78](https://github.com/Seidy06/Sistema_Licitaciones_XP/issues/78) | `iteracion-4/hu-37-tag-entrega` |

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
