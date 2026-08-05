# Historias de usuario

Este documento registra el backlog inicial del Sistema de Gestión de
Licitaciones. Las historias podrán refinarse, dividirse o repriorizarse durante
las iteraciones de acuerdo con la retroalimentación del cliente y el aprendizaje
del equipo.

> **Nota sobre las estimaciones:** los puntos indicados representan una
> propuesta inicial del equipo para apoyar la planificación. No son valores
> impuestos ni definitivos y podrán ajustarse mediante estimación colectiva
> cuando se refine el backlog.

Durante la primera iteración se implementará completamente la gestión de
proveedores. Las demás historias se registran desde el inicio para conservar una
visión integral del producto y facilitar la planificación de iteraciones
posteriores.

## Backlog inicial

| ID | Historia | Prioridad inicial | Estimación inicial |
| --- | --- | --- | ---: |
| HU-01 | Registrar proveedor | Alta | 3 puntos |
| HU-02 | Consultar proveedores | Alta | 2 puntos |
| HU-03 | Editar proveedor | Alta | 2 puntos |
| HU-04 | Eliminar proveedor | Media | 2 puntos |
| HU-05 | Crear licitación | Alta | 5 puntos |
| HU-06 | Publicar o cerrar licitación | Alta | 5 puntos |
| HU-07 | Registrar oferta | Alta | 5 puntos |
| HU-08 | Consultar mejor oferta | Alta | 5 puntos |
| HU-09 | Administrar niveles de aprobación | Media | 3 puntos |
| HU-10 | Administrar tipo de cambio | Media | 3 puntos |
| HU-11 | Alternar CRC y USD | Media | 3 puntos |
| HU-12 | Usar operaciones mediante API REST | Alta | 5 puntos |
| HU-13 | Usar modo claro y oscuro | Baja | 2 puntos |
| HU-14 | Ejecutar el sistema con Docker | Alta | 5 puntos |
| HU-15 | Desplegar el sistema en Kubernetes | Alta | 8 puntos |
| HU-16 | Consultar inicio y navegar por el sistema | Media | 3 puntos |
| HU-17 | Verificar el sistema con pruebas automatizadas | Alta | 8 puntos |
| HU-18 | Integrar cambios continuamente | Alta | 5 puntos |

## HU-01: Registrar proveedor

Como encargado de licitaciones,\
quiero registrar un proveedor,\
para asociarlo posteriormente con sus ofertas económicas.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 3 puntos\
**Iteración prevista:** 1

### Criterios de aceptación

1. El nombre del proveedor es obligatorio.
2. El nombre debe ser único después de normalizarlo.
3. Se eliminan los espacios al inicio y al final.
4. Se reducen los espacios repetidos a un solo espacio.
5. Se normalizan las diferencias entre mayúsculas y minúsculas para comprobar
   la unicidad.
6. Se aplica normalización Unicode.
7. Solo se aceptan letras, números, espacios, punto, coma y paréntesis.
8. El proveedor se almacena en PostgreSQL.
9. El registro está disponible desde la interfaz MVC y la API REST.
10. Un nombre duplicado produce un mensaje controlado.
11. La API devuelve `201 Created` cuando el registro es válido.
12. La API devuelve `409 Conflict` cuando el proveedor ya existe.

Las reglas de validación y unicidad deben aplicarse de forma coherente en la
interfaz, el servidor y PostgreSQL. Por ejemplo, `Empresa Central`,
`empresa central` y `EMPRESA CENTRAL` deben considerarse nombres equivalentes.

## HU-02: Consultar proveedores

Como encargado de licitaciones,\
quiero consultar los proveedores registrados,\
para identificarlos y utilizarlos en los procesos de contratación.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 2 puntos\
**Iteración prevista:** 1

### Criterios de aceptación

1. Se muestra una lista de los proveedores registrados.
2. Cada registro presenta, como mínimo, su identificador y nombre.
3. Es posible consultar un proveedor específico mediante su identificador.
4. Si no existen proveedores, se muestra un resultado vacío controlado.
5. Si el identificador no existe, la API devuelve `404 Not Found`.
6. La consulta está disponible desde la interfaz MVC y la API REST.
7. La información consultada proviene de PostgreSQL.
8. El listado permite paginar, filtrar y ordenar los resultados.

## HU-03: Editar proveedor

Como encargado de licitaciones,\
quiero editar el nombre de un proveedor,\
para mantener sus datos actualizados.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 2 puntos\
**Iteración prevista:** 1

### Criterios de aceptación

1. Es posible seleccionar un proveedor existente y modificar su nombre.
2. El nuevo nombre cumple las mismas reglas de validación y normalización
   definidas en HU-01.
3. No se permite cambiar el nombre por otro que pertenezca a un proveedor
   diferente después de normalizarlo.
4. La actualización se almacena en PostgreSQL.
5. La operación está disponible desde la interfaz MVC y la API REST.
6. Una actualización válida produce un mensaje controlado de confirmación.
7. Un nombre duplicado produce una respuesta controlada y la API devuelve
   `409 Conflict`.
8. Si el proveedor no existe, la API devuelve `404 Not Found`.

## HU-04: Eliminar proveedor

Como encargado de licitaciones,\
quiero eliminar un proveedor,\
para retirar registros que ya no deben utilizarse.

**Prioridad inicial:** Media\
**Estimación inicial propuesta:** 2 puntos\
**Iteración prevista:** 1

### Criterios de aceptación

1. Es posible solicitar la eliminación de un proveedor existente.
2. La interfaz solicita confirmación antes de ejecutar la eliminación.
3. El proveedor eliminado deja de aparecer en las consultas.
4. La eliminación se refleja en PostgreSQL.
5. La operación está disponible desde la interfaz MVC y la API REST.
6. Si el proveedor no existe, la API devuelve `404 Not Found`.
7. Si el proveedor está asociado con información que impide eliminarlo, el
   sistema conserva el registro y devuelve un mensaje controlado.

## HU-05: Crear licitación

Como encargado de licitaciones,\
quiero crear una licitación,\
para registrar una necesidad de contratación y recibir ofertas.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 5 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. Se registra, como mínimo, un código único, la descripción, el presupuesto en
   CRC y la fecha y hora de cierre.
2. El código es obligatorio y único después de eliminar espacios laterales y
   comparar sin distinguir mayúsculas y minúsculas.
3. El presupuesto es obligatorio, mayor que cero y se almacena como `decimal`
   con precisión explícita.
4. La fecha y hora de cierre se seleccionan mediante un control de calendario y
   hora.
5. Las fechas se procesan internamente en UTC y se presentan en la zona
   `America/Costa_Rica`.
6. La licitación recibe un identificador automático que el usuario no puede
   editar.
7. Una licitación nueva se crea en estado de borrador.
8. La licitación se almacena en PostgreSQL.
9. La creación está disponible desde la interfaz MVC y la API REST.
10. Los datos inválidos generan mensajes controlados y no se almacenan.
11. La edición no permite reducir el presupuesto por debajo de una oferta
    existente.
12. Las consultas, edición y eliminación completan el CRUD de licitaciones.
13. No se elimina físicamente una licitación con ofertas relacionadas, salvo
    que se aplique borrado lógico de manera consistente.

## HU-06: Publicar o cerrar licitación

Como encargado de licitaciones,\
quiero publicar o cerrar una licitación,\
para controlar el periodo en que se pueden recibir ofertas.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 5 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. Una licitación sigue el ciclo `Borrador` → `Publicada` → `Cerrada`.
2. Una licitación en borrador puede publicarse si contiene todos los datos
   obligatorios.
3. Una licitación publicada puede cerrarse.
4. No se aceptan transiciones de estado que incumplan el ciclo definido.
5. Al alcanzar su fecha y hora de cierre, una licitación se considera cerrada
   funcionalmente aunque su campo de estado todavía indique `Publicada`.
6. El estado actualizado se almacena en PostgreSQL.
7. La interfaz muestra el estado funcional vigente de la licitación.
8. Las operaciones están disponibles desde la interfaz MVC y la API REST.
9. Una transición inválida produce un mensaje controlado y no modifica el
   estado.

## HU-07: Registrar oferta

Como encargado de licitaciones,\
quiero registrar la oferta económica de un proveedor,\
para compararla con las demás propuestas recibidas.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 5 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. La oferta se asocia con una licitación y un proveedor existentes.
2. Solo se aceptan ofertas para licitaciones publicadas y no vencidas.
3. No se acepta una oferta cuando la fecha y hora actuales son iguales o
   posteriores a la fecha de cierre.
4. El monto es obligatorio, mayor que cero y se almacena oficialmente en CRC
   como `decimal` con precisión explícita.
5. La oferta no puede superar el presupuesto; una oferta igual al presupuesto
   sí es válida.
6. Un proveedor solo puede registrar una oferta por licitación. PostgreSQL
   protege esta regla con un índice único compuesto por licitación y proveedor.
7. La oferta se almacena en PostgreSQL.
8. El registro está disponible desde la interfaz MVC y la API REST.
9. Las consultas, edición y eliminación completan el CRUD de ofertas.
10. Las ofertas de licitaciones vencidas o cerradas no se pueden crear, editar
    ni eliminar y deben conservarse como evidencia.
11. Los datos o asociaciones inválidos producen mensajes controlados y no se
    almacenan.

## HU-08: Consultar mejor oferta

Como encargado de licitaciones,\
quiero consultar la mejor oferta de una licitación,\
para apoyar la selección de la propuesta más conveniente.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 5 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. El sistema compara las ofertas válidas asociadas con la licitación.
2. Se identifica como mejor oferta la de menor monto en CRC.
3. En caso de empate, se selecciona la oferta registrada primero.
4. El resultado muestra el proveedor, el monto en CRC, el porcentaje de ahorro,
   la clasificación y el nivel de aprobación.
5. El porcentaje de ahorro se calcula como
   `((Presupuesto CRC - Mejor oferta CRC) / Presupuesto CRC) × 100`.
6. Un ahorro igual o superior al 10 % se clasifica como
   `Oferta conveniente`.
7. Un ahorro mayor que 0 % y menor que 10 % se clasifica como
   `Oferta aceptable`.
8. Una oferta igual al presupuesto se clasifica como
   `Oferta válida sin ahorro`.
9. Si no existen ofertas válidas, se muestra `Sin ofertas válidas`.
10. La consulta está disponible desde la interfaz MVC y la API REST.

## HU-09: Administrar niveles de aprobación

Como administrador del sistema,\
quiero administrar los niveles de aprobación por monto,\
para aplicar el nivel de autorización correspondiente a cada licitación.

**Prioridad inicial:** Media\
**Estimación inicial propuesta:** 3 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. Se pueden crear, consultar, editar y eliminar niveles de aprobación.
2. Cada nivel define un nombre y el rango de montos al que aplica.
3. Los rangos deben ser válidos y no pueden solaparse.
4. Solo puede existir un rango abierto sin monto máximo.
5. El sistema determina el nivel correspondiente desde la configuración
   almacenada, sin una cadena fija de condiciones.
6. Los cambios se almacenan en PostgreSQL.
7. Las operaciones están disponibles desde la interfaz MVC y la API REST.
8. Las configuraciones inválidas producen mensajes controlados.

## HU-10: Administrar tipo de cambio

Como administrador del sistema,\
quiero administrar el tipo de cambio entre CRC y USD,\
para convertir y comparar montos expresados en monedas diferentes.

**Prioridad inicial:** Media\
**Estimación inicial propuesta:** 3 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. Se pueden crear, consultar, editar y eliminar tipos de cambio CRC por USD.
2. El valor del tipo de cambio debe ser mayor que cero.
3. El registro incluye la fecha correspondiente.
4. Solo puede existir un tipo de cambio activo para la operación ordinaria.
5. Es posible activar un tipo de cambio registrado.
6. El sistema funciona sin Internet utilizando la configuración local.
7. Los datos se almacenan en PostgreSQL.
8. Las operaciones están disponibles desde la interfaz MVC y la API REST.
9. Un valor inválido produce un mensaje controlado y no se almacena.

## HU-11: Alternar CRC y USD

Como usuario del sistema,\
quiero alternar la visualización de montos entre CRC y USD,\
para analizar la información en la moneda que necesite.

**Prioridad inicial:** Media\
**Estimación inicial propuesta:** 3 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. El usuario puede seleccionar CRC o USD como moneda de visualización.
2. CRC es la moneda oficial y la única fuente de verdad persistida.
3. El monto en USD se calcula como
   `Monto CRC / Tipo de cambio CRC por USD`.
4. Se muestra la fecha del tipo de cambio utilizado.
5. La moneda seleccionada se identifica claramente junto a cada monto.
6. Cambiar la visualización no altera los valores originales almacenados en
   CRC.
7. Los colones se presentan con el formato y la cultura `es-CR`.
8. Si no existe un tipo de cambio activo, se muestra un mensaje controlado.

## HU-12: Usar operaciones mediante API REST

Como sistema cliente,\
quiero utilizar las operaciones del sistema mediante una API REST,\
para integrar la gestión de licitaciones con otras aplicaciones.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 5 puntos\
**Iteración prevista:** Transversal

### Criterios de aceptación

1. La API expone el CRUD de proveedores, licitaciones, ofertas, niveles de
   aprobación y tipos de cambio, además de cambios de estado, activación del
   tipo de cambio y consulta de la mejor oferta.
2. Los recursos utilizan rutas bajo `/api/v1`.
3. Las solicitudes y respuestas utilizan JSON y contratos DTO; no se exponen
   directamente entidades de Entity Framework Core.
4. Los listados admiten paginación, filtrado y ordenamiento.
5. La API utiliza, cuando corresponde, `200 OK`, `201 Created`,
   `204 No Content`, `400 Bad Request`, `404 Not Found`, `409 Conflict` y
   `422 Unprocessable Entity`.
6. Los errores utilizan `ProblemDetails` con título, estado, detalle seguro,
   código de error e identificador de correlación.
7. Una respuesta de error no expone trazas, rutas internas, consultas,
   credenciales ni mensajes técnicos.
8. Las operaciones respetan las mismas reglas de negocio que la interfaz MVC.
9. OpenAPI/Swagger documenta las rutas, parámetros, contratos y respuestas.
10. Existe una colección reproducible de solicitudes documentada en `/docs`.

## HU-13: Usar modo claro y oscuro

Como usuario del sistema,\
quiero alternar entre modo claro y oscuro,\
para utilizar la interfaz con la apariencia que me resulte más cómoda.

**Prioridad inicial:** Baja\
**Estimación inicial propuesta:** 2 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. La interfaz permite seleccionar el modo claro o el modo oscuro.
2. El cambio se aplica a las vistas principales sin recargar información.
3. Los textos, controles y mensajes mantienen un contraste legible en ambos
   modos.
4. La preferencia se conserva durante la navegación.
5. El cambio de tema no altera los datos ni las operaciones del sistema.

## HU-14: Ejecutar el sistema con Docker

Como integrante del equipo de desarrollo,\
quiero ejecutar el sistema y sus dependencias con Docker,\
para disponer de un entorno reproducible.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 5 puntos\
**Iteración prevista:** Transversal

### Criterios de aceptación

1. La aplicación cuenta con una imagen de contenedor reproducible.
2. PostgreSQL se ejecuta como un servicio de contenedor.
3. Docker Compose permite iniciar los servicios requeridos con una sola orden.
4. La aplicación puede comunicarse con PostgreSQL dentro del entorno de
   contenedores.
5. La configuración sensible se suministra mediante variables de entorno o
   secretos y no queda incorporada en la imagen.
6. La documentación indica cómo construir, iniciar, comprobar y detener el
   entorno.
7. Los servicios exponen mecanismos que permiten comprobar su estado.
8. PostgreSQL utiliza un volumen y conserva los datos después de reiniciar los
   contenedores.
9. La imagen se construye con un Dockerfile multi-stage compatible con .NET 9.

## HU-15: Desplegar el sistema en Kubernetes

Como responsable de despliegue,\
quiero desplegar el sistema en Kubernetes,\
para ejecutarlo en un entorno orquestado y reproducible.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 8 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. Existen manifiestos o plantillas para desplegar la aplicación y sus recursos
   requeridos.
2. La configuración y los datos sensibles se administran por separado mediante
   `ConfigMap` y `Secret`, según corresponda.
3. La aplicación dispone de verificaciones de disponibilidad y estado.
4. Los recursos desplegados pueden comunicarse con PostgreSQL.
5. La aplicación se expone mediante un servicio de Kubernetes.
6. El despliegue puede aplicarse y eliminarse siguiendo instrucciones
   documentadas.
7. Después del despliegue, se puede comprobar el acceso a la aplicación y su
   funcionamiento básico.
8. PostgreSQL se despliega mediante un `StatefulSet` o mecanismo equivalente y
   utiliza un `PersistentVolumeClaim`.
9. La aplicación define startup, readiness y liveness probes, así como
   solicitudes y límites de recursos.
10. Las migraciones se ejecutan de forma controlada y se documenta evidencia de
    pods, servicios, almacenamiento, logs y conservación de datos.

## HU-16: Consultar inicio y navegar por el sistema

Como usuario del sistema,\
quiero consultar una página inicial y navegar entre los módulos,\
para comprender el flujo de licitaciones y acceder fácilmente a sus funciones.

**Prioridad inicial:** Media\
**Estimación inicial propuesta:** 3 puntos\
**Iteración prevista:** Por definir

### Criterios de aceptación

1. La página inicial explica el propósito de la aplicación.
2. La página inicial describe el flujo de licitación, las ofertas, la mejor
   oferta, el nivel de aprobación y la conversión monetaria.
3. Existe un menú visible con acceso a Inicio, Licitaciones, Proveedores,
   Ofertas, Niveles de aprobación, Tipo de cambio y la documentación
   interactiva de la API.
4. La navegación funciona en computadoras y dispositivos móviles.
5. Los mensajes de éxito, advertencia y error son visibles y comprensibles.
6. Los formularios muestran las validaciones junto al campo correspondiente.
7. Los recursos front-end están disponibles localmente o cuentan con un
   mecanismo documentado de respaldo si falla la CDN.

## HU-17: Verificar el sistema con pruebas automatizadas

Como integrante del equipo de desarrollo,\
quiero verificar las reglas y los flujos mediante pruebas automatizadas,\
para detectar regresiones y entregar incrementos confiables.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 8 puntos\
**Iteración prevista:** Transversal

### Criterios de aceptación

1. Las reglas de dominio y aplicación cuentan con pruebas unitarias.
2. Se prueban, como mínimo, valores monetarios, presupuesto, ofertas duplicadas,
   estados, vencimiento, normalización, unicidad, mejor oferta, desempate,
   ahorro, aprobación, conversión y transiciones.
3. Las pruebas de integración se ejecutan contra PostgreSQL real mediante
   Testcontainers o un mecanismo equivalente.
4. Las pruebas de integración verifican migraciones, índices, claves foráneas,
   restricciones, transacciones, concurrencia, persistencia y endpoints.
5. Playwright o Selenium verifica los flujos principales desde el navegador,
   incluido el CRUD, la navegación, los temas y la conversión monetaria.
6. Domain y Application alcanzan al menos 80 % de cobertura de líneas y el
   proyecto completo alcanza al menos 70 %.
7. Las pruebas se escriben o actualizan antes o junto con la implementación y
   dejan evidencia del ciclo rojo-verde-refactorización.
8. La estrategia, la forma de ejecución, la cobertura y los casos principales
   se documentan en `/docs/pruebas.md`.

## HU-18: Integrar cambios continuamente

Como integrante del equipo de desarrollo,\
quiero validar automáticamente cada cambio,\
para impedir la integración de código que incumpla los controles de calidad.

**Prioridad inicial:** Alta\
**Estimación inicial propuesta:** 5 puntos\
**Iteración prevista:** Transversal

### Criterios de aceptación

1. GitHub Actions restaura las dependencias y compila la solución.
2. El flujo ejecuta las pruebas automatizadas y recopila la cobertura.
3. El flujo comprueba el formato y el análisis estático.
4. El flujo construye la imagen Docker.
5. El flujo valida los manifiestos de Kubernetes.
6. El flujo revisa las dependencias en busca de vulnerabilidades.
7. Un fallo en cualquiera de los controles impide integrar el cambio.
8. El resultado de la integración continua queda disponible como evidencia de
   cada iteración.
