# Módulo de interfaz web

`Licitaciones.Web` usa ASP.NET Core MVC, vistas Razor, Bootstrap y validación unobtrusive. El incremento funcional de la Iteración 1 corresponde a proveedores; la Iteración 3 inicia la experiencia informativa con la landing page (HU-20).

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
