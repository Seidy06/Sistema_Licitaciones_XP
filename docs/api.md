# API REST implementada

La API de negocio de la Iteración 1 expone proveedores bajo `/api/v1/proveedores`. La Iteración 2 agrega licitaciones bajo `/api/v1/licitaciones`. Los ejemplos de identificadores, fechas y versiones son ilustrativos. El proyecto conserva además `GET /WeatherForecast`, generado por la plantilla; es un endpoint de muestra y no forma parte del dominio de licitaciones.

## Resumen

| Método y ruta | Resultado exitoso | Errores controlados |
| --- | --- | --- |
| `GET /api/v1/proveedores` | `200 OK` | No hay un contrato de error personalizado para esta acción. |
| `GET /api/v1/proveedores/{id}` | `200 OK` | `404 Not Found`. |
| `POST /api/v1/proveedores` | `201 Created` | `400 Bad Request`, `409 Conflict`. |
| `PUT /api/v1/proveedores/{id}` | `200 OK` | `400 Bad Request`, `404 Not Found`, `409 Conflict`. |
| `DELETE /api/v1/proveedores/{id}` | `204 No Content` | `404 Not Found`. |
| `GET /api/v1/proveedores/historico` | `200 OK` | No hay un contrato de error personalizado para esta acción. |
| `GET /api/v1/proveedores/historico/{id}` | `200 OK` | `404 Not Found`. |
| `GET /api/v1/licitaciones` | `200 OK` | No hay un contrato de error personalizado para esta acción. |
| `GET /api/v1/licitaciones/{id}` | `200 OK` | `404 Not Found`. |
| `POST /api/v1/licitaciones` | `201 Created` | `400 Bad Request`, `409 Conflict`. |
| `POST /api/v1/ofertas` | `201 Created` | `400 Bad Request`, `409 Conflict`, `422 Unprocessable Entity`. |
| `PUT /api/v1/ofertas/{id}` | No modifica la oferta. | `422 Unprocessable Entity`. |
| `DELETE /api/v1/ofertas/{id}` | No elimina la oferta. | `422 Unprocessable Entity`. |

## Listar y consultar

`GET /api/v1/proveedores` acepta `pagina` (1), `tamanoPagina` (20, máximo 100), `nombre`, `ordenarPor` (`Nombre` o `FechaCreacion`) y `descendente` (`false`).

```http
GET /api/v1/proveedores?pagina=1&tamanoPagina=10&nombre=central&ordenarPor=Nombre&descendente=false
```

```json
{"items":[],"total":0,"pagina":1,"tamanoPagina":10}
```

`GET /api/v1/proveedores/{id}` devuelve el DTO siguiente o 404:

```json
{
  "id": "7d9413f2-2bde-4bc9-af45-39a66f8fcce5",
  "nombre": "Empresa Central",
  "nombreNormalizado": "EMPRESA CENTRAL",
  "createdAt": "2026-08-15T12:00:00+00:00",
  "updatedAt": "2026-08-15T12:00:00+00:00",
  "version": 1
}
```

## Registrar

```http
POST /api/v1/proveedores
Content-Type: application/json

{"nombre":"Empresa Central"}
```

Devuelve `201 Created`, el DTO y una cabecera `Location` hacia el detalle. Un nombre inválido devuelve 400; un nombre activo equivalente devuelve 409 con `ProblemDetails` titulado `Proveedor duplicado`.

## Editar

```http
PUT /api/v1/proveedores/7d9413f2-2bde-4bc9-af45-39a66f8fcce5
Content-Type: application/json

{"nombre":"Empresa Central Actualizada","version":1}
```

Devuelve `200 OK` con el DTO actualizado. Devuelve 404 si no existe un proveedor activo; 409 por nombre duplicado o versión desactualizada; y 400 por nombre inválido. `version` corresponde al token `xmin` serializado como entero sin signo.

## Dar de baja

`DELETE /api/v1/proveedores/{id}` establece `DeletedAt`: no elimina la fila. Devuelve 204 sin cuerpo o 404 si el proveedor activo no existe. Tras la baja, el listado y el detalle ordinarios dejan de encontrarlo.

## Histórico

`GET /api/v1/proveedores/historico` acepta los mismos parámetros de paginación,
filtro y ordenamiento del listado activo, pero devuelve exclusivamente
proveedores dados de baja. `GET /api/v1/proveedores/historico/{id}` permite
consultar su detalle e incluye `deletedAt`. Estas rutas son explícitas para no
debilitar el filtro global aplicado al resto de las consultas.

La aplicación registra OpenAPI y publica el documento solo en Development con `MapOpenApi()`. No existe Swagger UI en esta iteración.

## Licitaciones (HU-13)

### Listar

`GET /api/v1/licitaciones` retorna todas las licitaciones activas con su
estado efectivo computado (cierre funcional incluido).

```http
GET /api/v1/licitaciones
```

```json
{
  "items": [
    {
      "id": "d5d2f6a1-...",
      "titulo": "Compra de material informático",
      "presupuesto": 10000.00,
      "fechaCierre": "2026-08-25T12:00:00+00:00",
      "estadoEfectivo": "Publicada"
    }
  ]
}
```

### Consultar detalle

`GET /api/v1/licitaciones/{id}` devuelve el DTO de detalle o 404. El detalle
incluye mejor oferta (monto mínimo de ofertas recibidas) y nivel de aprobación
correspondiente. Si no existen ofertas, ambos campos son `null`.

```http
GET /api/v1/licitaciones/d5d2f6a1-...
```

```json
{
  "id": "d5d2f6a1-...",
  "codigo": "COMP-2026-001",
  "titulo": "Compra de material informático",
  "presupuesto": 10000.00,
  "fechaCierre": "2026-08-25T12:00:00+00:00",
  "mejorOferta": { "monto": 8000.00 },
  "nivelAprobacion": { "id": 2, "nombre": "Gerencial" }
}
```

### Crear licitación (HU-10)

`POST /api/v1/licitaciones` crea una licitación en estado `Borrador`.
Devuelve `201 Created` con el DTO y cabecera `Location`. Un código duplicado
devuelve 409; datos inválidos devuelven 400.

## Registrar oferta (HU-14)

`POST /api/v1/ofertas` registra una oferta para una licitación publicada y no
vencida.

```http
POST /api/v1/ofertas
Content-Type: application/json

{
  "licitacionId": "d5d2f6a1-0000-0000-0000-000000000001",
  "proveedorId": "7d9413f2-0000-0000-0000-000000000002",
  "monto": 8000.00
}
```

Una solicitud válida devuelve `201 Created`, una cabecera `Location` con
`/api/v1/ofertas/{id}` y el DTO siguiente:

```json
{
  "id": "9a3d94d0-0000-0000-0000-000000000003",
  "licitacionId": "d5d2f6a1-0000-0000-0000-000000000001",
  "proveedorId": "7d9413f2-0000-0000-0000-000000000002",
  "monto": 8000.00,
  "fechaRegistro": "2026-08-19T15:00:00+00:00"
}
```

Devuelve `409 Conflict` con título `Oferta duplicada` cuando el proveedor ya
tiene una oferta para la licitación. Devuelve `422 Unprocessable Entity` con
título `Oferta rechazada` cuando la licitación está vencida o el monto supera
el presupuesto. Devuelve `400 Bad Request` para los demás rechazos controlados,
como licitación inexistente o no publicada y proveedor inexistente. La
validación automática del contrato también responde 400 cuando los
identificadores o el monto no satisfacen sus restricciones.

## Proteger ofertas registradas (HU-15)

`PUT /api/v1/ofertas/{id}` y `DELETE /api/v1/ofertas/{id}` rechazan cambios
sobre ofertas registradas con `422 Unprocessable Entity`. Para una oferta
asociada a una licitación cerrada, el `ProblemDetails` usa el título `Oferta
inalterable` y explica que no puede editarse ni eliminarse. Estas rutas no
persisten cambios: la oferta se conserva como evidencia con su licitación,
proveedor y monto originales.
