# Historias de Usuario — Sistema de Gestión de Licitaciones (XP)

> Metodología: Extreme Programming (XP). Cada historia sigue el formato de Planning Game: **Rol / Quiero / Para**, con **prioridad**, **estimación en puntos de historia (SP)**, **criterios de aceptación** en formato Given/When/Then y **notas técnicas** dirigidas a un agente de código. Las historias están agrupadas por *release* (entrega incremental), respetando el orden de "Entregas mínimas identificables" del enunciado.

Prioridad: `Alta` = bloqueante para releases posteriores | `Media` | `Baja`.
Estimación: escala Fibonacci (1, 2, 3, 5, 8, 13).

---

## Adopción del catálogo y trazabilidad histórica

Este catálogo HU-00 a HU-37 es la referencia aprobada para continuar el
proyecto. La auditoría de la Iteración 1 utilizó una numeración anterior para
las historias de proveedores; la evidencia producida se interpreta mediante
la siguiente equivalencia:

| Historia auditada anteriormente | Historia del catálogo actual |
| --- | --- |
| HU-01 — Registrar proveedor | HU-06 — Registrar proveedor |
| HU-02 — Consultar proveedores | HU-09 — Listar y consultar proveedores |
| HU-03 — Editar proveedor | HU-07 — Editar proveedor |
| HU-04 — Eliminar proveedor | HU-08 — Eliminar lógicamente proveedor |

Los commits históricos no se modificarán ni se reescribirán porque forman
parte de la evidencia XP del repositorio. En particular, la implementación
auditada de registro de proveedores conserva sus referencias al Issue #8, al
Pull Request #9, a la rama `hu/01-proveedores` y a los commits `f274b20`,
`f597141`, `0fd4129`, `89f0768` y `1516d2c`. Esta trazabilidad se conserva sin
alterar los identificadores originales y se relaciona actualmente con HU-06.

---

## RELEASE 0 — Inicialización y Planificación XP

### HU-00: Inicializar repositorio y estructura del proyecto
- **Rol:** Como equipo de desarrollo
- **Quiero:** tener la estructura base del repositorio (solución .NET 9, carpetas `/src`, `/tests`, `/docs`, `/k8s`, `/docker`) con Git y `.gitignore` configurados
- **Para:** iniciar el desarrollo incremental con una base limpia y trazable
- **Prioridad:** Alta | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given un repositorio vacío, When se ejecuta el setup inicial, Then existe una solución `.sln` con proyectos `Domain`, `Application`, `Infrastructure`, `Web` (MVC) y `Api`.
  - Given la estructura creada, When se revisa el repositorio, Then existen carpetas `/docs`, `/docs/assets`, `/k8s`, `/tests` vacías o con placeholders.
  - Given el repositorio, When se inspecciona `.gitignore`, Then excluye `bin/`, `obj/`, `.env`, secretos y artefactos generados.
- **Notas técnicas:** Crear solución con `dotnet new sln`, proyectos con `dotnet new classlib` (Domain, Application, Infrastructure), `dotnet new mvc` (Web) y `dotnet new webapi` (Api). Configurar referencias entre proyectos: Web/Api → Application → Domain; Infrastructure → Application/Domain.

### HU-01: Documentar plan de release XP e historias iniciales
- **Rol:** Como equipo de desarrollo
- **Quiero:** registrar en `/docs/plan-xp.md` el plan de liberaciones, iteraciones y reglas de trabajo XP, y en `/docs/historias-usuario.md` el catálogo de historias con prioridad y estimación
- **Para:** dejar evidencia verificable del proceso XP exigido por el proyecto
- **Prioridad:** Alta | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given el archivo `/docs/plan-xp.md`, When se abre, Then contiene releases, iteraciones cortas, prácticas XP aplicadas (Planning Game, TDD, refactorización, integración continua, propiedad colectiva) y velocidad planificada.
  - Given `/docs/historias-usuario.md`, When se abre, Then contiene todas las historias con ID, prioridad, estimación y criterios de aceptación.
- **Notas técnicas:** Este mismo documento puede copiarse como base de `/docs/historias-usuario.md`.

---

## RELEASE 1 — Dominio, Modelo de Datos y Persistencia

### HU-02: Modelar entidades de dominio
- **Rol:** Como desarrollador
- **Quiero:** definir las entidades `Proveedor`, `Licitacion`, `Oferta`, `NivelAprobacion`, `TipoCambio` y `EstadoLicitacion` en la capa `Domain`
- **Para:** contar con un modelo de dominio rico, sin dependencias de infraestructura, que exprese las reglas de negocio
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given la capa Domain, When se compila, Then no tiene referencias a Entity Framework Core ni a ASP.NET.
  - Given la entidad `Proveedor`, When se instancia, Then el `Id` es generado internamente (Guid) y no editable desde fuera del constructor/factory.
  - Given la entidad `Oferta`, When se crea con monto negativo o cero, Then lanza una excepción de dominio (`DomainException`/`BusinessRuleException`).
  - Given los montos monetarios, When se declaran los tipos, Then usan `decimal` (nunca `float`/`double`).
- **Notas técnicas:** Aplicar patrón de entidades ricas (constructores privados + métodos de fábrica `Crear(...)`), value objects para `Dinero` (monto en CRC) si se desea, y `enum EstadoLicitacion { Borrador, Publicada, Cerrada, Adjudicada, Cancelada }` reflejado también en tabla parametrizable en BD.

### HU-03: Configurar EF Core 9 y contexto de base de datos con PostgreSQL
- **Rol:** Como desarrollador
- **Quiero:** configurar `AppDbContext` con Npgsql (proveedor de PostgreSQL) y mapeos Fluent API para todas las entidades
- **Para:** persistir el dominio en PostgreSQL respetando tipos, precisión decimal y restricciones
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given el `AppDbContext`, When se configura `Oferta.Monto`, Then el tipo de columna es `numeric(18,2)`.
  - Given la cadena de conexión, When se lee en tiempo de ejecución, Then proviene de variables de entorno/`appsettings` con soporte para secretos, no hardcodeada.
  - Given el contexto, When se agregan `CreatedAt`/`UpdatedAt`, Then se establecen automáticamente mediante `SaveChanges` interceptado (no manual en cada servicio).
- **Notas técnicas:** Usar `Npgsql.EntityFrameworkCore.PostgreSQL`. Configurar `HasColumnType("timestamptz")` para fechas y `DateTimeOffset` en el modelo. Implementar `SaveChangesInterceptor` o sobreescribir `SaveChangesAsync` para timestamps y versión de concurrencia.

### HU-04: Crear migraciones iniciales y datos semilla
- **Rol:** Como desarrollador
- **Quiero:** generar la migración inicial de EF Core y datos semilla para `EstadoLicitacion`, `NivelAprobacion` y `TipoCambio`
- **Para:** tener una base de datos reproducible desde cero en cualquier entorno
- **Prioridad:** Alta | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given una base de datos vacía, When se ejecuta `dotnet ef database update`, Then se crean todas las tablas, índices únicos, llaves foráneas y restricciones CHECK descritas en el modelo de datos.
  - Given la migración aplicada, When se consultan las tablas de catálogo, Then existen los 5 estados de licitación, al menos 3 niveles de aprobación sin traslape y un tipo de cambio activo inicial.
- **Notas técnicas:** Usar `HasData` en `OnModelCreating` o un `DbContext` seeder ejecutado en el arranque (solo en desarrollo/CI, nunca sobreescribiendo datos productivos). Referenciar el script `database-schema.sql` como fuente de verdad para índices y constraints avanzados (exclusion constraints) que EF Core no genera nativamente.

### HU-05: Abstraer el reloj del sistema (Clock Service)
- **Rol:** Como desarrollador
- **Quiero:** un servicio `IClock`/`ISystemClock` inyectable que exponga la fecha/hora actual en UTC
- **Para:** permitir pruebas deterministas de vencimiento de licitaciones y ofertas
- **Prioridad:** Alta | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given un servicio que valida vencimiento, When se inyecta `IClock`, Then no usa `DateTime.Now`/`DateTime.UtcNow` directamente.
  - Given una prueba unitaria, When se usa un `FakeClock` con fecha fija, Then el resultado de la validación es determinista y reproducible.
- **Notas técnicas:** Interfaz `IClock { DateTimeOffset UtcNow(); }`. Implementación real `SystemClock` registrada en DI; implementación de prueba `FixedClock` en el proyecto de tests.

---

## RELEASE 2 — Proveedores

### HU-06: Registrar proveedor
- **Rol:** Como usuario administrador
- **Quiero:** registrar un proveedor indicando su nombre
- **Para:** poder asociarlo posteriormente a ofertas de licitaciones
- **Prioridad:** Alta | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given un nombre válido y único, When se registra el proveedor, Then se persiste con `Id` autogenerado y `CreatedAt` establecido.
  - Given un nombre con espacios repetidos o mayúsculas/minúsculas distintas a uno ya existente tras normalización, When se intenta registrar, Then se rechaza con código 409/mensaje "proveedor duplicado".
  - Given dos representaciones Unicode canónicamente equivalentes, When se registran, Then ambas producen el mismo `NombreNormalizado` y la segunda se rechaza como duplicada.
  - Given dos solicitudes concurrentes con nombres equivalentes, When ambas superan la consulta previa, Then el índice único permite una sola inserción y la otra solicitud recibe `409 Conflict`, nunca `500 Internal Server Error`.
  - Given un nombre con caracteres no permitidos (fuera de letras, números, espacios, `.`, `,`, `(` y `)`), When se intenta registrar, Then se rechaza con mensaje de validación específico.
  - Given la validación del nombre, When se procesa una solicitud MVC o API, Then las reglas se ejecutan en Domain sin duplicarse en los controladores ni en sus modelos de entrada.
- **Notas técnicas:** `ProveedorNombreNormalizer` es la única estrategia de normalización: aplica Unicode Form C, `Trim()`, colapsa espacios múltiples y usa `ToUpperInvariant()` para `NombreNormalizado`. PostgreSQL protege la unicidad mediante `UX_Proveedores_NombreNormalizado`; Infrastructure traduce específicamente su violación `23505` a `ProveedorDuplicadoException`.

### HU-07: Editar proveedor
- **Rol:** Como usuario administrador
- **Quiero:** editar el nombre de un proveedor existente
- **Para:** corregir datos manteniendo la integridad de unicidad
- **Prioridad:** Media | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given un proveedor existente, When se edita a un nombre que normalizado colisiona con otro proveedor activo, Then se rechaza la edición.
  - Given una edición concurrente con datos desactualizados (versión distinta), When se guarda, Then se detecta `DbUpdateConcurrencyException` y se informa al usuario para refrescar.
- **Notas técnicas:** Reutilizar el mismo validador/normalizador de HU-06. Implementar columna `Version`/`xmin` como concurrency token en EF Core (`IsRowVersion()` o `IsConcurrencyToken()`).

### HU-08: Eliminar (lógicamente) proveedor
- **Rol:** Como usuario administrador
- **Quiero:** eliminar un proveedor, con confirmación previa
- **Para:** dar de baja proveedores sin perder trazabilidad de ofertas históricas
- **Prioridad:** Media | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given un proveedor sin ofertas asociadas, When se elimina, Then se aplica borrado lógico (`DeletedAt` establecido) tras confirmación explícita del usuario.
  - Given un proveedor con ofertas relacionadas, When se intenta eliminar físicamente, Then el sistema lo impide y traduce el error de integridad referencial a un mensaje controlado.
  - Given un proveedor eliminado lógicamente, When se lista, Then no aparece en listados activos pero sí es consultable en reportes/histórico si se solicita explícitamente.
- **Notas técnicas:** Filtro global de EF Core (`HasQueryFilter(p => p.DeletedAt == null)`) para excluir eliminados por defecto.

### HU-09: Listar y consultar proveedores
- **Rol:** Como usuario
- **Quiero:** listar proveedores con paginación, filtro por nombre y ordenamiento
- **Para:** ubicar proveedores fácilmente en catálogos grandes
- **Prioridad:** Media | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given una lista de proveedores, When se solicita una página con tamaño N, Then se retorna solo esa porción junto con metadatos de paginación (total, página actual).
  - Given un filtro de texto, When se aplica, Then solo se muestran proveedores cuyo nombre lo contenga (case-insensitive).
  - Given un criterio de ordenamiento (nombre, fecha de creación), When se aplica, Then los resultados respetan el orden solicitado.
- **Notas técnicas:** Implementar en capa Application vía patrón de repositorio/specification o `IQueryable` con `Skip/Take`. Reutilizar el mismo contrato de paginación para todos los módulos (proveedores, licitaciones, ofertas).

---

## RELEASE 3 — Licitaciones

### HU-10: Crear licitación
- **Rol:** Como usuario administrador
- **Quiero:** crear una licitación con código único, título, presupuesto y fecha/hora de cierre seleccionada mediante calendario
- **Para:** iniciar el proceso de recepción de ofertas
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given un código ya existente (ignorando espacios laterales y mayúsculas/minúsculas), When se intenta crear, Then se rechaza como duplicado.
  - Given un presupuesto menor o igual a cero, When se intenta crear, Then se rechaza en cliente, servidor y base de datos (CHECK).
  - Given la fecha de cierre, When se captura en el formulario, Then se usa un control de calendario y hora (no texto libre).
  - Given una licitación nueva, When se persiste, Then su estado inicial es `Borrador`.
- **Notas técnicas:** `CodigoNormalizado` = `Trim().ToUpperInvariant()`, con índice único filtrado por `DeletedAt IS NULL`. Persistir `FechaCierre` como `timestamptz`/`DateTimeOffset`; el formulario en MVC debe usar un input `datetime-local` o date-picker + time-picker JS, convirtiendo a UTC antes de enviar.

### HU-11: Publicar licitación
- **Rol:** Como usuario administrador
- **Quiero:** publicar una licitación en estado `Borrador`
- **Para:** habilitarla a recibir ofertas de proveedores
- **Prioridad:** Alta | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given una licitación en `Borrador`, When se publica, Then transiciona a `Publicada` y se registra la transición (tabla `licitacion_transiciones`).
  - Given una licitación en cualquier estado distinto de `Borrador`, When se intenta publicar, Then se rechaza indicando la transición inválida.
  - Given una licitación cuya `FechaCierre` ya pasó, When se intenta publicar, Then se rechaza (no se puede publicar algo ya vencido).
- **Notas técnicas:** Implementar máquina de estados explícita en el dominio (`Licitacion.Publicar()`), no condicionales dispersos en la capa de aplicación. Registrar `EstadoAnteriorId`, `EstadoNuevoId`, `Fecha` en `licitacion_transiciones`.

### HU-12: Editar y cerrar licitación
- **Rol:** Como usuario administrador
- **Quiero:** editar datos permitidos de una licitación y cerrarla manual o automáticamente por vencimiento
- **Para:** mantener el flujo de licitación consistente con la realidad temporal
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given una licitación cuya `FechaCierre` ya se alcanzó, When se consulta su estado efectivo, Then el sistema la trata como cerrada funcionalmente aunque el campo `Estado` todavía almacene `Publicada` (regla de "cierre funcional").
  - Given un presupuesto que se intenta reducir por debajo de una oferta ya registrada, When se guarda la edición, Then se rechaza.
  - Given una licitación cerrada (formal o funcionalmente), When se intenta editar campos protegidos (código, presupuesto, fecha de cierre), Then se rechaza.
- **Notas técnicas:** Implementar un método de dominio/servicio `EstadoEfectivo(IClock clock)` que compare `FechaCierre` contra `clock.UtcNow()` sin depender de un job batch obligatorio (aunque puede complementarse con uno). La comparación siempre en UTC; la presentación en `America/Costa_Rica`.

### HU-13: Listar y consultar licitaciones
- **Rol:** Como usuario
- **Quiero:** listar licitaciones con paginación, filtro (por estado, código, rango de fechas) y ordenamiento
- **Para:** dar seguimiento al proceso de licitación
- **Prioridad:** Media | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given un filtro por estado efectivo (incluyendo cierre funcional), When se aplica, Then el listado refleja el estado real, no solo el campo persistido.
  - Given el detalle de una licitación, When se consulta, Then muestra la mejor oferta actual, su clasificación y el nivel de aprobación correspondiente.
- **Notas técnicas:** Reutilizar el cálculo de mejor oferta de HU-16 en el detalle.

---

## RELEASE 4 — Ofertas

### HU-14: Registrar oferta
- **Rol:** Como usuario (en representación de un proveedor)
- **Quiero:** registrar una oferta económica de un proveedor para una licitación publicada
- **Para:** participar en el proceso de licitación
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given una licitación no publicada o cerrada (formal o funcionalmente), When se intenta registrar una oferta, Then se rechaza con mensaje claro.
  - Given la fecha/hora actual igual o posterior a la fecha de cierre, When se intenta registrar, Then se rechaza (oferta vencida), usando `IClock` para la comparación.
  - Given un proveedor que ya tiene una oferta activa para la misma licitación, When intenta registrar otra, Then se rechaza como oferta duplicada (validado también por índice único compuesto `LicitacionId + ProveedorId`).
  - Given un monto de oferta superior al presupuesto, When se intenta registrar, Then se rechaza; un monto igual al presupuesto es aceptado.
  - Given un monto menor o igual a cero, When se intenta registrar, Then se rechaza en cliente, servidor y base de datos.
- **Notas técnicas:** Encapsular todas las reglas en un método de dominio/servicio de aplicación `RegistrarOferta(...)` que use `IClock` y valide en este orden: estado de licitación → vencimiento → duplicidad → presupuesto → monto positivo, devolviendo errores específicos por regla (no un mensaje genérico) para facilitar las pruebas unitarias (HU-33).

### HU-15: Rechazar y auditar ofertas inválidas
- **Rol:** Como sistema
- **Quiero:** intentar registrar ofertas inválidas (duplicada, superior al presupuesto, vencida) y verificar su rechazo explícito
- **Para:** garantizar la integridad del proceso de licitación (flujo funcional mínimo exigido por el proyecto)
- **Prioridad:** Alta | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given los tres escenarios (duplicada, sobre presupuesto, vencida), When se ejecutan como pruebas de integración/funcionales, Then todos son rechazados con el código HTTP y mensaje correspondiente (409/422).
  - Given una oferta cerrada (perteneciente a una licitación cerrada), When se intenta editar o eliminar, Then se rechaza siempre, conservándose como evidencia inalterable.
- **Notas técnicas:** Esta historia formaliza como prueba explícita el "Flujo funcional mínimo" del enunciado; no introduce entidades nuevas, solo casos de prueba end-to-end sobre HU-14.

### HU-16: Calcular mejor oferta y clasificación de ahorro
- **Rol:** Como usuario
- **Quiero:** consultar la mejor oferta de una licitación, su clasificación de ahorro y el nivel de aprobación correspondiente
- **Para:** tomar decisiones informadas sobre la adjudicación
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given varias ofertas válidas, When se calcula la mejor oferta, Then es la de menor monto en CRC; en caso de empate, se selecciona la registrada primero (por `FechaRegistro`).
  - Given ninguna oferta válida, When se consulta, Then se muestra "Sin ofertas válidas".
  - Given un ahorro ≥ 10% respecto al presupuesto, When se clasifica, Then se etiqueta "Oferta conveniente".
  - Given un ahorro > 0% y < 10%, When se clasifica, Then se etiqueta "Oferta aceptable".
  - Given una oferta igual al presupuesto, When se clasifica, Then se etiqueta "Oferta válida sin ahorro".
- **Notas técnicas:** `Ahorro % = (Presupuesto - MejorOferta) / Presupuesto * 100`. Implementar como servicio de dominio puro (`CalculadoraMejorOferta`) fácilmente testeable sin dependencias de infraestructura.

### HU-17: Listar y consultar ofertas
- **Rol:** Como usuario
- **Quiero:** listar ofertas de una licitación con paginación, filtro y ordenamiento
- **Para:** revisar el detalle de participación de proveedores
- **Prioridad:** Media | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given una licitación con ofertas, When se listan, Then se muestra proveedor, monto (CRC y USD alternable), fecha de registro y si es la mejor oferta.
- **Notas técnicas:** Reutilizar el servicio de conversión CRC/USD de HU-19.

---

## RELEASE 5 — Niveles de Aprobación y Conversión Monetaria

### HU-18: Administrar niveles de aprobación (tabla parametrizable)
- **Rol:** Como usuario administrador
- **Quiero:** crear, editar, listar y desactivar niveles de aprobación (rango de monto mínimo/máximo y aprobador)
- **Para:** que el sistema determine el aprobador según el monto sin lógica if/else fija
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given dos niveles con rangos que se traslapan, When se intenta guardar el segundo, Then se rechaza en servidor y en base de datos (exclusion constraint).
  - Given un nivel sin monto máximo (rango abierto), When ya existe otro nivel abierto activo, Then se rechaza la creación de un segundo rango abierto.
  - Given un monto de oferta/adjudicación, When se resuelve el aprobador, Then se obtiene consultando la tabla `niveles_aprobacion`, nunca mediante una cadena de `if/else` en código.
- **Notas técnicas:** El servicio `ResolverNivelAprobacion(decimal monto)` debe hacer una consulta parametrizada (`WHERE :monto >= monto_minimo AND (monto_maximo IS NULL OR :monto <= monto_maximo) AND activo`). La integridad de no traslape se refuerza a nivel de base de datos con `EXCLUDE USING gist` (ver script SQL).

### HU-19: Administrar tipo de cambio y conversión CRC/USD
- **Rol:** Como usuario administrador
- **Quiero:** administrar el tipo de cambio CRC→USD y que la interfaz permita alternar la visualización de montos
- **Para:** ofrecer una referencia en dólares sin alterar los valores oficiales almacenados en colones
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given un nuevo tipo de cambio activado, When se guarda, Then se desactiva automáticamente cualquier otro tipo de cambio previamente activo (solo uno activo a la vez).
  - Given un monto almacenado en CRC, When el usuario alterna a USD, Then se calcula `monto / tipoCambio.valor` solo para presentación, sin modificar el valor persistido.
  - Given la vista con montos en USD, When se muestra, Then también se muestra la fecha del tipo de cambio utilizado.
  - Given ausencia de conexión a Internet, When se usa el sistema, Then la conversión sigue funcionando con el tipo de cambio administrado localmente (sin dependencia de API externa).
- **Notas técnicas:** Índice único parcial `WHERE activo = true` garantiza unicidad del tipo de cambio activo a nivel de BD. El botón de alternancia CRC/USD debe ser un componente de interfaz reutilizable (JS) que no dispare nuevas llamadas al servidor si el valor ya está en el DOM (cálculo en cliente usando el tipo de cambio activo obtenido junto con los datos).

---

## RELEASE 6 — MVC, Landing Page y Experiencia de Usuario

### HU-20: Landing page informativa
- **Rol:** Como visitante
- **Quiero:** ver una landing page que explique el propósito de la aplicación, el flujo de licitación, ofertas, mejor oferta, nivel de aprobación y conversión monetaria
- **Para:** entender el sistema antes de operarlo
- **Prioridad:** Media | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given la ruta raíz `/`, When se accede sin autenticación, Then se muestra la landing con las secciones explicativas indicadas.
  - Given un dispositivo móvil, When se visualiza la landing, Then el diseño es responsivo (Bootstrap o equivalente).
- **Notas técnicas:** Vista Razor estática/parcialmente dinámica en el proyecto `Web` (MVC), sin lógica de negocio en el controlador (controlador delgado).

### HU-21: Menú de navegación global
- **Rol:** Como usuario
- **Quiero:** un menú visible con acceso a Inicio, Licitaciones, Proveedores, Ofertas, Niveles de aprobación, Tipo de cambio y documentación interactiva de la API
- **Para:** navegar fácilmente entre los módulos del sistema
- **Prioridad:** Media | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given cualquier página del sitio, When se renderiza el layout, Then el menú está presente y resalta la sección activa.
  - Given el enlace a documentación de API, When se hace clic, Then abre Swagger UI.
- **Notas técnicas:** Layout compartido `_Layout.cshtml` con partial view de navegación.

### HU-22: Modo claro/oscuro persistente
- **Rol:** Como usuario
- **Quiero:** alternar entre modo claro y oscuro con un control visible
- **Para:** adaptar la interfaz a mi preferencia visual
- **Prioridad:** Baja | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given el control de tema, When se cambia, Then la preferencia persiste entre sesiones (almacenamiento local del navegador).
  - Given una nueva visita, When se carga la página, Then se respeta el último tema seleccionado.
- **Notas técnicas:** Uso de `localStorage` + clase CSS en `<html>`/`<body>`; variables CSS para paleta de colores.

### HU-23: CRUD completo desde la interfaz web para todos los módulos
- **Rol:** Como usuario administrador
- **Quiero:** crear, leer, actualizar y eliminar proveedores, licitaciones, ofertas, niveles de aprobación y tipos de cambio desde vistas MVC
- **Para:** operar el sistema completamente desde el navegador sin depender de la API directamente
- **Prioridad:** Alta | **Estimación:** 8 SP
- **Criterios de aceptación:**
  - Given cada módulo, When se accede a su vista de listado, Then soporta paginación, filtrado y ordenamiento (tablas).
  - Given cada formulario, When se envía con datos inválidos, Then se muestran mensajes de validación junto al campo correspondiente, sin recargar toda la información ya ingresada.
  - Given cualquier eliminación permitida, When se solicita, Then el sistema pide confirmación antes de ejecutarla.
- **Notas técnicas:** Controladores MVC delgados que delegan en servicios de `Application`. Usar `ViewModels`/`DTOs` específicos por vista, nunca las entidades de dominio directamente en las vistas.

### HU-24: Mensajería de éxito, advertencia y error
- **Rol:** Como usuario
- **Quiero:** recibir mensajes claros de éxito, advertencia y error tras cada operación
- **Para:** entender el resultado de mis acciones
- **Prioridad:** Media | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given una operación exitosa, When se completa, Then se muestra un mensaje de confirmación (toast/alert).
  - Given un error de negocio (regla violada), When ocurre, Then el mensaje es específico y comprensible (no un stack trace).
- **Notas técnicas:** Middleware/filtro de excepciones en MVC que traduzca `DomainException` a `TempData`/`ModelState` con mensajes amigables.

### HU-25: Formato monetario y cultural es-CR
- **Rol:** Como usuario
- **Quiero:** ver los montos en colones con el formato cultural costarricense
- **Para:** interpretar correctamente las cifras monetarias
- **Prioridad:** Baja | **Estimación:** 1 SP
- **Criterios de aceptación:**
  - Given un monto en CRC, When se presenta en cualquier vista, Then usa separador de miles y formato `es-CR` (ej. `₡1.500.000,00`).
- **Notas técnicas:** Configurar `CultureInfo("es-CR")` en el pipeline de localización de ASP.NET Core o formatear en el helper de vista.

---

## RELEASE 7 — API REST

### HU-26: Exponer API REST con DTOs y versionado
- **Rol:** Como cliente externo (integrador)
- **Quiero:** consumir una API REST versionada para proveedores, licitaciones, ofertas, niveles de aprobación y tipo de cambio
- **Para:** integrar el sistema con otras plataformas sin depender de la interfaz web
- **Prioridad:** Alta | **Estimación:** 8 SP
- **Criterios de aceptación:**
  - Given cualquier endpoint, When retorna datos, Then usa DTOs específicos (nunca entidades EF Core expuestas directamente).
  - Given la ruta base, When se define, Then incluye versión (`/api/v1/...`).
  - Given operaciones CRUD, When se ejecutan, Then retornan los códigos HTTP correctos: 200, 201, 204, 400, 404, 409, 422 y 500 controlado.
  - Given cualquier error, When se retorna, Then usa el formato `ProblemDetails` (título, estado, detalle seguro, código de error, identificador de correlación), sin exponer stack traces, rutas internas ni secretos.
  - Given listados, When se solicitan, Then soportan paginación, filtrado y ordenamiento vía query params.
- **Notas técnicas:** Usar `Asp.Versioning`, `ProblemDetailsFactory` personalizado, middleware global de manejo de excepciones que mapee `DomainException` → 422, `NotFoundException` → 404, `ConflictException` → 409.

### HU-27: Documentación interactiva OpenAPI/Swagger
- **Rol:** Como desarrollador/integrador
- **Quiero:** acceder a documentación interactiva de la API (Swagger UI) con ejemplos
- **Para:** conocer los contratos, probar endpoints y entender los errores posibles
- **Prioridad:** Media | **Estimación:** 2 SP
- **Criterios de aceptación:**
  - Given la ruta `/swagger`, When se accede, Then se muestra la documentación generada con todos los endpoints, esquemas de request/response y ejemplos.
  - Given `/docs/api.md`, When se abre, Then documenta endpoints, contratos, ejemplos y errores, y referencia una colección reproducible de solicitudes (Postman/Insomnia/`.http`).
- **Notas técnicas:** `Swashbuckle.AspNetCore` con `XML comments` habilitados (`GenerateDocumentationFile`). Incluir colección `.http`/Postman en `/docs`.

---

## RELEASE 8 — Pruebas (TDD)

### HU-28: Configurar TDD y pipeline de pruebas unitarias del dominio
- **Rol:** Como equipo de desarrollo
- **Quiero:** configurar el proyecto de pruebas unitarias (xUnit) y aplicar el ciclo rojo-verde-refactorización para las reglas de negocio
- **Para:** garantizar corrección y evidencia disciplinada de TDD según XP
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given cada regla de negocio (presupuesto/oferta > 0, oferta duplicada, oferta sobre presupuesto, estado no publicado, vencimiento, normalización de proveedor, código único, mejor oferta y desempate, clasificación de ahorro, nivel de aprobación, conversión CRC/USD, transiciones de estado), When se implementa, Then existe al menos una prueba unitaria previa o concurrente que la cubre.
  - Given la capa Domain/Application, When se mide cobertura, Then alcanza al menos 80% de líneas.
- **Notas técnicas:** Proyecto `Tests.Unit` con xUnit + FluentAssertions. Usar `FixedClock` (HU-05) para pruebas de vencimiento. Ejecutar `dotnet test /p:CollectCoverage=true` o `coverlet`.

### HU-29: Pruebas de integración contra PostgreSQL real
- **Rol:** Como equipo de desarrollo
- **Quiero:** ejecutar pruebas de integración contra una instancia real de PostgreSQL en contenedor (Testcontainers)
- **Para:** validar migraciones, índices únicos, llaves foráneas, restricciones y concurrencia con el motor real (no SQLite ni mocks)
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given el proyecto `Tests.Integration`, When se ejecuta, Then levanta un contenedor PostgreSQL real vía Testcontainers y aplica las migraciones.
  - Given un intento de insertar un código de licitación duplicado directamente vía EF Core, When se ejecuta, Then la base de datos rechaza la operación (constraint violation capturada y traducida).
  - Given una prueba de concurrencia optimista, When dos actualizaciones concurrentes ocurren sobre el mismo registro, Then la segunda lanza `DbUpdateConcurrencyException`.
- **Notas técnicas:** Paquete `Testcontainers.PostgreSql`. No usar SQLite in-memory en ningún proyecto de pruebas.

### HU-30: Pruebas funcionales de extremo a extremo (E2E)
- **Rol:** Como equipo de desarrollo
- **Quiero:** automatizar pruebas E2E con Playwright/Selenium cubriendo landing page, CRUD de proveedores/licitaciones/ofertas, modo claro/oscuro, conversión CRC/USD y mensajes de validación
- **Para:** verificar el flujo funcional mínimo completo desde el navegador
- **Prioridad:** Alta | **Estimación:** 8 SP
- **Criterios de aceptación:**
  - Given el flujo funcional mínimo descrito en el proyecto (registrar proveedor → crear licitación → publicar → registrar oferta → verificar rechazos → consultar mejor oferta → alternar CRC/USD), When se ejecuta como prueba E2E, Then todos los pasos pasan de forma automatizada.
  - Given el pipeline de CI, When se ejecutan las pruebas E2E, Then corren contra la aplicación levantada en modo headless.
- **Notas técnicas:** Playwright .NET recomendado por su integración nativa con GitHub Actions. Ejecutar contra la app en Docker Compose dentro del job de CI.

---

## RELEASE 9 — Docker y Docker Compose

### HU-31: Contenerizar la aplicación con Dockerfile multi-stage
- **Rol:** Como equipo de desarrollo
- **Quiero:** un `Dockerfile` multi-stage compatible con .NET 9 que compile y ejecute la aplicación con un usuario no privilegiado
- **Para:** obtener una imagen reproducible y segura
- **Prioridad:** Alta | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given el `Dockerfile`, When se construye, Then usa una etapa `build` con SDK y una etapa `runtime` con ASP.NET runtime únicamente.
  - Given el contenedor final, When se ejecuta, Then corre con un usuario no root (cuando la imagen base lo permita).
  - Given health checks, When se configuran, Then exponen un endpoint `/health` verificable por Docker/Kubernetes.
- **Notas técnicas:** Imagen base `mcr.microsoft.com/dotnet/sdk:9.0` (build) y `mcr.microsoft.com/dotnet/aspnet:9.0` (runtime). Usar `HEALTHCHECK` o endpoint de `AddHealthChecks()`.

### HU-32: Orquestar entorno local con Docker Compose
- **Rol:** Como desarrollador
- **Quiero:** un `docker-compose.yml` con el servicio de aplicación y PostgreSQL, volumen persistente, variables de entorno y health checks
- **Para:** levantar el entorno completo con `docker compose up --build` de forma reproducible
- **Prioridad:** Alta | **Estimación:** 3 SP
- **Criterios de aceptación:**
  - Given `docker compose up --build`, When se ejecuta desde cero, Then la aplicación y PostgreSQL inician correctamente y la app aplica migraciones automáticamente o mediante un job de inicialización.
  - Given un reinicio de contenedores, When ocurre, Then los datos persisten gracias al volumen configurado para PostgreSQL.
  - Given `/docs/docker.md`, When se abre, Then documenta instrucciones reproducibles de uso.
- **Notas técnicas:** Servicio `db` con imagen `postgres:16`, volumen nombrado, variables `POSTGRES_USER/PASSWORD/DB` vía `.env` (no versionado). Servicio `app` con `depends_on` + `condition: service_healthy`.

---

## RELEASE 10 — Kubernetes

### HU-33: Manifiestos de despliegue de la aplicación
- **Rol:** Como equipo de desarrollo
- **Quiero:** un `Deployment`, `Service`, `ConfigMap` y `Secret` para la aplicación
- **Para:** desplegarla en un clúster de Kubernetes con configuración segura
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given el `Deployment`, When se aplica, Then define `startupProbe`, `readinessProbe` y `livenessProbe`, además de `resources.requests/limits`.
  - Given credenciales de base de datos, When se referencian, Then provienen de un `Secret`, nunca hardcodeadas en el manifiesto.
  - Given el `Service`, When se crea, Then expone el puerto de la aplicación dentro del clúster.
- **Notas técnicas:** Carpeta `/k8s` con `deployment.yaml`, `service.yaml`, `configmap.yaml`, `secret.yaml`.

### HU-34: Manifiestos de persistencia de PostgreSQL en Kubernetes
- **Rol:** Como equipo de desarrollo
- **Quiero:** un `StatefulSet` (o mecanismo equivalente) para PostgreSQL con `PersistentVolumeClaim`
- **Para:** garantizar persistencia de datos entre reinicios de pods en el clúster
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given el `StatefulSet` de PostgreSQL, When un pod se reinicia, Then los datos se conservan gracias al `PersistentVolumeClaim`.
  - Given las migraciones, When se ejecutan en el clúster, Then se aplican de forma controlada (Job/InitContainer), no automáticamente en cada arranque de réplica.
  - Given `/docs/kubernetes.md`, When se abre, Then documenta instrucciones reproducibles y evidencia de pods, servicios, PVC, logs y conservación de datos tras reinicio.
- **Notas técnicas:** Usar `volumeClaimTemplates` en el `StatefulSet`. Job de migración con `dotnet ef database update` o `dotnet-ef bundle` ejecutado como `Job` de Kubernetes antes del despliegue de la app (o `initContainer`).

---

## RELEASE 11 — Integración Continua (GitHub Actions)

### HU-35: Pipeline de CI con build, pruebas, análisis y Docker
- **Rol:** Como equipo de desarrollo
- **Quiero:** un workflow de GitHub Actions que restaure dependencias, compile, ejecute pruebas con cobertura, valide formato/análisis estático, construya la imagen Docker, valide manifiestos de Kubernetes y revise dependencias vulnerables
- **Para:** bloquear la integración de cambios que rompan la calidad o el despliegue (integración continua exigida por XP)
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given un push o pull request, When se dispara el workflow, Then ejecuta en orden: restore → build → test (con cobertura) → análisis estático/formato → build de imagen Docker → validación de manifiestos K8s → auditoría de dependencias.
  - Given cualquier paso fallido, When ocurre, Then el workflow falla y bloquea el merge (branch protection).
- **Notas técnicas:** `dotnet format --verify-no-changes`, `dotnet list package --vulnerable`, `kubeval`/`kubeconform` para validar manifiestos, `docker build` sin publicar (o publicar a registry solo en tags de release).

---

## RELEASE 12 — Documentación y Cierre

### HU-36: Documentación técnica completa en /docs
- **Rol:** Como equipo de desarrollo
- **Quiero:** completar `/docs/README.md`, `integracion-modulos.md`, `arquitectura-general.md`, `modelo-datos.md` (con diagramas Mermaid), `pruebas.md`, `bitacora-xp.md` y `uso-ia.md`
- **Para:** cumplir con la documentación mínima requerida y declarar el uso responsable de herramientas de IA
- **Prioridad:** Alta | **Estimación:** 5 SP
- **Criterios de aceptación:**
  - Given `/docs/README.md`, When se abre, Then funciona como índice de navegación de toda la documentación.
  - Given `arquitectura-general.md` y `modelo-datos.md`, When se abren, Then incluyen diagramas Mermaid o imágenes en `/docs/assets`.
  - Given `bitacora-xp.md`, When se abre, Then registra resultados, velocidad, retroalimentación, ciclos TDD, refactorizaciones y pequeñas liberaciones por iteración.
  - Given `uso-ia.md`, When se abre, Then declara herramienta, finalidad, módulos asistidos, ejemplos y validaciones realizadas por el equipo.
- **Notas técnicas:** No se requiere documentación fuera de `/docs`; este archivo sustituye el README tradicional en la raíz.

### HU-37: Etiquetado de entrega final
- **Rol:** Como equipo de desarrollo
- **Quiero:** etiquetar la entrega evaluable con `v1.0.0` o `entrega-final`
- **Para:** identificar de forma inequívoca la versión a evaluar
- **Prioridad:** Alta | **Estimación:** 1 SP
- **Criterios de aceptación:**
  - Given el repositorio, When se lista los tags, Then existe `v1.0.0` (o `entrega-final`) apuntando al commit final funcional.
  - Given el historial de commits, When se revisa, Then muestra distribución equilibrada entre ambos integrantes de la pareja, con mensajes descriptivos vinculados a historias.
- **Notas técnicas:** `git tag -a v1.0.0 -m "Entrega final"` y `git push --tags`.

---

## Resumen de estimación por release

| Release | Historias | SP totales aprox. |
|---|---|---|
| 0. Inicialización y planificación | HU-00, HU-01 | 4 |
| 1. Dominio y persistencia | HU-02 a HU-05 | 15 |
| 2. Proveedores | HU-06 a HU-09 | 11 |
| 3. Licitaciones | HU-10 a HU-13 | 16 |
| 4. Ofertas | HU-14 a HU-17 | 15 |
| 5. Aprobación y conversión | HU-18, HU-19 | 10 |
| 6. MVC y landing page | HU-20 a HU-25 | 18 |
| 7. API REST | HU-26, HU-27 | 10 |
| 8. Pruebas (TDD) | HU-28 a HU-30 | 18 |
| 9. Docker | HU-31, HU-32 | 6 |
| 10. Kubernetes | HU-33, HU-34 | 10 |
| 11. CI (GitHub Actions) | HU-35 | 5 |
| 12. Documentación y cierre | HU-36, HU-37 | 6 |
| **Total** | **38 historias** | **~144 SP** |
