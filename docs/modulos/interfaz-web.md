# Módulo de interfaz web

`Licitaciones.Web` usa ASP.NET Core MVC, vistas Razor, Bootstrap y validación unobtrusive. El incremento funcional de la Iteración 1 corresponde a proveedores; la Iteración 3 incorpora la experiencia informativa con la landing page (HU-20), la navegación global (HU-21), el modo claro/oscuro persistente (HU-22), el CRUD web de HU-23, la mensajería unificada de éxito y error de HU-24 y el formato monetario es-CR de HU-25.

| Ruta MVC | Función |
| --- | --- |
| `GET /` | Landing informativa sin autenticación (HU-20). |
| `GET /Proveedores` | Listar, filtrar, ordenar y paginar. |
| `GET /Proveedores/Details/{id}` | Mostrar detalle activo. |
| `GET/POST /Proveedores/Create` | Mostrar formulario y registrar. |
| `GET/POST /Proveedores/Edit/{id}` | Mostrar formulario y editar con versión. |
| `GET /Proveedores/Delete/{id}` | Solicitar confirmación de baja. |
| `POST /Proveedores/DeleteConfirmed/{id}` | Ejecutar la baja lógica. |
| `GET /Proveedores/History` | Listar y filtrar proveedores dados de baja. |
| `GET /Proveedores/HistoryDetails/{id}` | Consultar el detalle histórico. |
| `GET /Licitaciones` | Listar, filtrar, ordenar y paginar licitaciones. |
| `GET/POST /Licitaciones/Create` | Mostrar formulario y registrar una licitación. |
| `GET /Ofertas?licitacionId={id}` | Listar, filtrar, ordenar y paginar ofertas de una licitación. |
| `GET/POST /Ofertas/Create` | Mostrar formulario y registrar una oferta. |
| `GET /NivelesAprobacion` | Listar, filtrar, ordenar y paginar niveles activos. |
| `GET/POST /NivelesAprobacion/Create` | Mostrar formulario y crear un nivel de aprobación. |
| `GET /NivelesAprobacion/Delete/{id}` | Solicitar confirmación de desactivación. |
| `POST /NivelesAprobacion/DeleteConfirmed/{id}` | Ejecutar la desactivación. |
| `GET /TiposCambio` | Listar, ordenar y paginar tipos de cambio. |
| `GET/POST /TiposCambio/Create` | Mostrar formulario y registrar un tipo de cambio. |

Los POST usan token antifalsificación. Los errores de nombre se presentan en el modelo, los conflictos de concurrencia en el resumen y los identificadores inexistentes producen 404. Tras crear o editar se usa `TempData` para el mensaje de éxito, que las vistas muestran mediante el partial compartido `_Mensajes` (HU-24).

Web aplica las migraciones de EF Core durante el arranque. Este comportamiento
puede desactivarse con `Database:ApplyMigrationsOnStartup=false` para hosts de
prueba que no acceden a persistencia. La ruta raíz `/` presenta desde HU-20 la
landing informativa: una vista Razor estática (`Views/Home/Index.cshtml`)
servida por `HomeController.Index` sin lógica de negocio, accesible sin
autenticación, que explica el propósito de la aplicación, el flujo de
licitación, las ofertas, la mejor oferta, el nivel de aprobación y la
conversión monetaria mediante tarjetas en una rejilla responsiva Bootstrap
(`col-12 col-md-6 col-xl-4`). La plantilla conserva además las páginas base
Home y Privacy. HU-23 añade las vistas Razor de listado y creación para
licitaciones, ofertas, niveles de aprobación y tipos de cambio, y las vistas de
detalle, edición, histórico y baja lógica ya existentes para proveedores.

## Navegación global (HU-21)

El layout compartido `Views/Shared/_Layout.cshtml` incluye el partial
`Views/Shared/_NavegacionGlobal.cshtml` en todas las páginas MVC. El menú ofrece
enlaces a Inicio (`/`), Licitaciones (`/Licitaciones`), Proveedores
(`/Proveedores`), Ofertas (`/Ofertas`), Niveles de aprobación
(`/NivelesAprobacion`), Tipo de cambio (`/TiposCambio`) y la documentación
interactiva de la API (`/swagger/index.html`). La sección activa se determina a
partir de la ruta HTTP actual y se marca con la clase `active`.

La navegación solo proporciona los enlaces globales; las operaciones CRUD de
HU-23 se implementan en sus controladores y vistas específicos. La evidencia de
los criterios de navegación está en las pruebas funcionales de
`NavegacionGlobalWebTests`; la evidencia del CRUD está documentada en la
sección HU-23 de `docs/pruebas.md`.

## Modo claro y oscuro persistente (HU-22)

El partial de navegación incluye un control visible (`id="theme-toggle"`, con
`aria-label`) para alternar entre modo claro y oscuro. El layout declara en el
`<head>` un script inicial que lee la última preferencia desde
`localStorage.getItem('theme')` y aplica el atributo `data-bs-theme` sobre el
elemento `<html>` antes del primer render, evitando el parpadeo del tema
incorrecto. `wwwroot/js/site.js` maneja el clic del control: calcula el tema
siguiente a partir del atributo actual, lo guarda con
`localStorage.setItem('theme', …)` con los valores `light`/`dark` y lo aplica al
documento; la preferencia persiste entre sesiones del navegador. La paleta del
modo oscuro se define en `wwwroot/css/site.css` mediante el selector
`[data-bs-theme='dark']` con las variables `--lic-superficie`, `--lic-texto` y
`--lic-borde`, que ajustan barra de navegación y pie de página.

La funcionalidad es cliente-side: no agrega rutas, controladores ni cambios de
persistencia de servidor. La evidencia de los dos criterios de aceptación está
en las pruebas funcionales de `TemaClaroOscuroWebTests`. El ícono del control es
fijo y no refleja el tema activo; ese refinamiento visual quedó registrado como
candidato a Issue separada.

## Mensajería de éxito y error (HU-24)

El partial compartido `Views/Shared/_Mensajes.cshtml` centraliza la
retroalimentación de las operaciones MVC. Cuando un controlador fija
`TempData["MensajeExito"]`, el partial lo muestra como alerta Bootstrap
dismisible (`alert alert-success`) en la vista destino, de modo que el mensaje
sobrevive a la redirección posterior a crear, eliminar o desactivar. Cuando el
`ModelState` contiene errores, el resumen `asp-validation-summary="ModelOnly"`
se presenta dentro de una alerta `alert-danger`: los mensajes provienen de las
reglas de dominio y aplicación (por ejemplo, traslape de rangos de niveles de
aprobación) y son específicos y comprensibles, sin exponer stack traces.

El partial está incluido en nueve vistas de los cinco módulos:
`Licitaciones/Index`, `Ofertas/Index`, `Ofertas/Create`,
`NivelesAprobacion/Index`, `NivelesAprobacion/Create`, `Proveedores/Create`,
`Proveedores/Edit`, `TiposCambio/Index` y `TiposCambio/Create`. `Program.cs` registra un
`HtmlEncoder` con los rangos `BasicLatin` y `Latin1Supplement` para que los
mensajes con acentos se muestren correctamente. La evidencia de los criterios
está en las pruebas de integración de `MensajeriaWebTests`; la variante de
advertencia del título de la historia aún no tiene flujos que produzcan
`TempData["MensajeAdvertencia"]` y quedó registrada como candidato a Issue
separada.

## Formato monetario es-CR (HU-25)

Los montos en colones de las vistas MVC usan el helper `FormatoMonetario`
(`src/Licitaciones.Web/FormatoMonetario.cs`), con métodos de extensión
`Dinero()` sobre `decimal` y `decimal?`. El helper clona la cultura `es-CR`,
fija el separador de miles en `.`, congela la instancia con
`CultureInfo.ReadOnly` y formatea con el patrón moneda, produciendo valores como
`₡1.500.000,00` de forma determinista independiente de la cultura del servidor.

El formato se aplica en los listados de licitaciones (`Presupuesto`),
ofertas (`Monto`) y niveles de aprobación (`MontoMinimo` y `MontoMaximo`, este
último con el texto alternativo "Sin límite" cuando es nulo). Para que el
símbolo ₡ (fuera de Latin-1) no se escape como entidad HTML, `Program.cs`
amplió el `HtmlEncoder` registrado con `UnicodeRanges.CurrencySymbols`.

La vista de confirmación `NivelesAprobacion/Delete` todavía presenta sus montos
con `.ToString("N2")`; extender allí el helper quedó registrado como candidato
a Issue separada. La evidencia del criterio está en las pruebas de integración
de `FormatoMonetarioWebTests` (HU-25).
