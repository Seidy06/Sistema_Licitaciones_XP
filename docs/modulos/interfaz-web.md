# Módulo de interfaz web

`Licitaciones.Web` usa ASP.NET Core MVC, vistas Razor, Bootstrap y validación unobtrusive. El incremento funcional de la Iteración 1 corresponde a proveedores; la Iteración 3 incorpora la experiencia informativa con la landing page (HU-20), la navegación global (HU-21), el modo claro/oscuro persistente (HU-22) y el CRUD web de HU-23.

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

Los POST usan token antifalsificación. Los errores de nombre se presentan en el modelo, los conflictos de concurrencia en el resumen y los identificadores inexistentes producen 404. Tras crear o editar se usa `TempData` para el mensaje de éxito.

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
