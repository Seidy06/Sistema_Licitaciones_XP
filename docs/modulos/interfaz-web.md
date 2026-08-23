# Módulo de interfaz web

`Licitaciones.Web` usa ASP.NET Core MVC, vistas Razor, Bootstrap y validación unobtrusive. El incremento funcional de la Iteración 1 corresponde a proveedores; la Iteración 3 inicia la experiencia informativa con la landing page (HU-20), la navegación global (HU-21) y el modo claro/oscuro persistente (HU-22).

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
Home y Privacy; siguen sin existir interfaces funcionales para licitaciones,
ofertas, niveles de aprobación ni tipos de cambio (corresponden a HU-23).

## Navegación global (HU-21)

El layout compartido `Views/Shared/_Layout.cshtml` incluye el partial
`Views/Shared/_NavegacionGlobal.cshtml` en todas las páginas MVC. El menú ofrece
enlaces a Inicio (`/`), Licitaciones (`/Licitaciones`), Proveedores
(`/Proveedores`), Ofertas (`/Ofertas`), Niveles de aprobación
(`/NivelesAprobacion`), Tipo de cambio (`/TiposCambio`) y la documentación
interactiva de la API (`/swagger/index.html`). La sección activa se determina a
partir de la ruta HTTP actual y se marca con la clase `active`.

La navegación solo proporciona los enlaces globales; no crea interfaces MVC
adicionales para módulos cuyo CRUD web corresponde a HU-23. La evidencia de los
dos criterios de aceptación está en las pruebas funcionales de
`NavegacionGlobalWebTests`.

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
