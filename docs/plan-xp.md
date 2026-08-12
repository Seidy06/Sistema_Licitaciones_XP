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

### Iteración 2: HU-10 a HU-17

- Creación, publicación, edición, cierre, listado y consulta de licitaciones.
- Registro, validación, auditoría, evaluación, listado y consulta de ofertas.
- TDD y refactorización de las reglas de negocio del incremento.

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
