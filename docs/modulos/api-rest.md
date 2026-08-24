# Módulo API REST

`Licitaciones.Api` es una aplicación ASP.NET Core 9 con controladores que expone
los cinco módulos del dominio bajo rutas versionadas `/api/v1/...`:
`proveedores` (CRUD lógico más `historico`), `licitaciones`
(listado, detalle, creación, edición, publicación y cierre), `ofertas`
(registro con protección e inmutabilidad, listado y detalle),
`niveles-aprobacion` (creación y resolución de aprobador) y `tipos-cambio`
(registro y consulta del activo). También permanece `GET /WeatherForecast`,
endpoint de muestra de la plantilla, sin relación con el dominio.

Los controladores reciben contratos HTTP (`Licitaciones.Api.Contracts`) e
invocan servicios de Application; retornan DTOs específicos (`ProveedorDto`,
`LicitacionDto`, `OfertaDto`, `PaginaResultado<T>`, …) y nunca exponen
entidades de EF Core.

## Contrato de errores (HU-26)

Toda respuesta de error usa `application/problem+json` con el formato
`ProblemDetails`: `title`, `status`, `detail` seguro y comprensible,
`type`, `instance` y dos extensiones obligatorias:

- `codigoError`: identificador estable del error (por ejemplo
  `error_http_404`, `regla_negocio_no_procesable`, `error_interno` o los
  códigos específicos que fijan los controladores).
- `correlacionId`: `TraceIdentifier` de la solicitud para seguimiento.

El contrato se aplica en un único punto por capa:

- `FabricaProblemDetailsApi` está registrada como `ProblemDetailsFactory`
  global y agrega las extensiones y títulos/detalles por defecto a todo
  problema generado por MVC (incluido el 404 sin cuerpo explícito).
- `RespuestaProblema.Crear` construye los problemas que los controladores
  devuelven al traducir excepciones esperadas.
- El manejador global de `UseExceptionHandler` mapea `DomainException` a
  `422` (`regla_negocio_no_procesable`) y cualquier excepción no prevista a un
  `500` controlado (`error_interno`), reutilizando la fábrica registrada; en
  ningún caso se exponen stack traces, rutas internas ni secretos.

Las respuestas exitosas conservan los códigos 200/201/204 según la operación,
y los listados aceptan paginación, filtrado y ordenamiento vía query params.

## Documentación interactiva (HU-27)

En Development la API expone Swagger UI en `/swagger` y su documento OpenAPI
en `/swagger/v1/swagger.json`, generados con Swashbuckle a partir de los
controladores y contratos existentes. El documento incluye comentarios XML
(`GenerateDocumentationFile`) y ejemplos por esquema mediante
`EjemplosEsquemasFiltro` (DTOs de respuesta y contratos de solicitud). Las
solicitudes también son reproducibles con la colección `docs/api.http`. En
otros entornos la documentación interactiva no está disponible.

## Operación

La API registra `AddProblemDetails()` y el manejador de excepciones. En
Development publica además el documento OpenAPI y la interfaz descritos arriba.
No hay autenticación ni autorización por roles. El proceso de API no ejecuta
migraciones: requiere una cadena `ConnectionStrings:Licitaciones` válida y una
base ya migrada.
