# Módulo API REST

`Licitaciones.Api` es una aplicación ASP.NET Core 9 con controladores. Expone el CRUD lógico de proveedores en `/api/v1/proveedores` (GET de listado y detalle, POST, PUT y DELETE, además de consultas explícitas bajo `/api/v1/proveedores/historico`) y la creación de licitaciones en `POST /api/v1/licitaciones` (HU-10). También permanece `GET /WeatherForecast`, endpoint de muestra de la plantilla, sin relación con el dominio.

El controlador recibe modelos HTTP para crear y editar, invoca servicios de Application y devuelve `ProveedorDto`; no expone entidades de EF Core. Los errores controlados de creación, edición y baja usan `ProblemDetails`. El 404 del detalle se devuelve sin cuerpo personalizado.

La API registra `AddProblemDetails()` y el manejador de excepciones. En Development publica el documento OpenAPI generado por `MapOpenApi()`. No hay Swagger UI, autenticación ni autorización por roles. No existen endpoints de publicación de licitaciones, ofertas, aprobaciones ni tipos de cambio.

El proceso de API no ejecuta migraciones. Requiere una cadena `ConnectionStrings:Licitaciones` válida y una base ya migrada.
