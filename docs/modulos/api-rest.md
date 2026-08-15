# Módulo API REST

`Licitaciones.Api` es una aplicación ASP.NET Core 9 con controladores. En la Iteración 1 su única API de negocio es el CRUD lógico de proveedores en `/api/v1/proveedores`: GET de listado y detalle, POST, PUT y DELETE. También permanece `GET /WeatherForecast`, endpoint de muestra de la plantilla, sin relación con el dominio.

El controlador recibe modelos HTTP para crear y editar, invoca servicios de Application y devuelve `ProveedorDto`; no expone entidades de EF Core. Los errores controlados de creación, edición y baja usan `ProblemDetails`. El 404 del detalle se devuelve sin cuerpo personalizado.

La API registra `AddProblemDetails()` y el manejador de excepciones. En Development publica el documento OpenAPI generado por `MapOpenApi()`. No hay Swagger UI, autenticación, autorización por roles ni endpoints de licitaciones, ofertas, aprobaciones o tipos de cambio en esta iteración.

El proceso de API no ejecuta migraciones. Requiere una cadena `ConnectionStrings:Licitaciones` válida y una base ya migrada.
