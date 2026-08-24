# API REST implementada

## Correcciones de cierre HU-11, HU-12, HU-13 y HU-17

- `POST /api/v1/licitaciones/{id}/publicar` publica un borrador.
- `PUT /api/v1/licitaciones/{id}` edita parcialmente una licitacion y
  `POST /api/v1/licitaciones/{id}/cerrar` realiza su cierre manual.
- El listado de licitaciones acepta `estadoFiltro`, `codigo`, `fechaDesde`,
  `fechaHasta`, `ordenarPor`, `descendente`, `pagina` y `tamanoPagina`.
- El listado de ofertas acepta `proveedor`, `ordenarPor`, `descendente`,
  `pagina` y `tamanoPagina`. Ambos listados responden con `items`, `total`,
  `pagina` y `tamanoPagina`.

La API de negocio de la Iteración 1 expone proveedores bajo `/api/v1/proveedores`. La Iteración 2 agrega licitaciones bajo `/api/v1/licitaciones`. Los ejemplos de identificadores, fechas y versiones son ilustrativos. El proyecto conserva además `GET /WeatherForecast`, generado por la plantilla; es un endpoint de muestra y no forma parte del dominio de licitaciones.

## Documentación interactiva (HU-27)

La API genera su documentación con `Swashbuckle.AspNetCore`. En Development, Swagger UI está disponible en [`/swagger/index.html`](http://localhost:5033/swagger/index.html) y el documento OpenAPI en `/swagger/v1/swagger.json`; incluye todos los endpoints, esquemas de request/response (`ProveedorDto`, `LicitacionDto`, `OfertaDto`, `TipoCambioDto`, `ProblemDetails`, `ValidationProblemDetails`) y ejemplos por esquema. La generación del archivo XML de comentarios está habilitada con `GenerateDocumentationFile`.

Las solicitudes son reproducibles con la colección [`docs/api.http`](api.http) (formato `.http`, compatible con Visual Studio Code REST Client, Rider, JetBrains HTTP client e importable desde Postman/Insomnia). Cubre los cinco recursos del dominio con sus casos exitosos y de error.

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
| `GET /api/v1/ofertas` | `200 OK` | `400 Bad Request`. |
| `GET /api/v1/ofertas/{id}` | `200 OK` | `400 Bad Request`, `404 Not Found`. |
| `POST /api/v1/ofertas` | `201 Created` | `400 Bad Request`, `409 Conflict`, `422 Unprocessable Entity`. |
| `PUT /api/v1/ofertas/{id}` | No modifica la oferta. | `422 Unprocessable Entity`. |
| `DELETE /api/v1/ofertas/{id}` | No elimina la oferta. | `422 Unprocessable Entity`. |
| `POST /api/v1/niveles-aprobacion` | `201 Created` | `400 Bad Request`, `409 Conflict`. |
| `GET /api/v1/niveles-aprobacion/resolver` | `200 OK` | `404 Not Found`. |
| `POST /api/v1/tipos-cambio` | `201 Created` | `400 Bad Request`. |
| `GET /api/v1/tipos-cambio/activo` | `200 OK` | `404 Not Found`. |

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

La aplicación registra OpenAPI con `AddOpenApi()`/`MapOpenApi()` (`/openapi/v1.json`) y, en Development, Swagger UI con Swashbuckle en `/swagger` (HU-27).

## Licitaciones (HU-13 y HU-16)

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
incluye la mejor oferta, su porcentaje y clasificación de ahorro, y el nivel de
aprobación correspondiente. La mejor oferta es la de menor monto; un empate se
resuelve por la `FechaRegistro` más temprana.

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
  "mejorOferta": {
    "id": "9a3d94d0-0000-0000-0000-000000000003",
    "monto": 8000.00,
    "ahorroPorcentaje": 20.0,
    "clasificacion": "Oferta conveniente"
  },
  "mensajeMejorOferta": null,
  "nivelAprobacion": { "id": 1, "nombre": "Operativo" }
}
```

La clasificación es `Oferta conveniente` para ahorro mayor o igual a 10 %,
`Oferta aceptable` para ahorro mayor que 0 % y menor que 10 %, y `Oferta válida
sin ahorro` cuando el monto coincide con el presupuesto. Si no existen ofertas,
`mejorOferta` y `nivelAprobacion` son `null`, mientras
`mensajeMejorOferta` contiene `Sin ofertas válidas`.

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

## Listar y consultar ofertas (HU-17)

`GET /api/v1/ofertas` requiere `licitacionId` y acepta `moneda`, cuyo valor
predeterminado es `CRC`. Retorna las ofertas de la licitación ordenadas por
monto y fecha de registro. Cada elemento incluye el nombre del proveedor y el
indicador `esMejorOferta`; la mejor es la de menor monto y un empate se resuelve
por la fecha de registro más temprana.

```http
GET /api/v1/ofertas?licitacionId=d5d2f6a1-0000-0000-0000-000000000001&moneda=CRC
```

```json
[
  {
    "id": "9a3d94d0-0000-0000-0000-000000000003",
    "proveedorNombre": "Empresa Central",
    "monto": 8000.00,
    "moneda": "CRC",
    "fechaRegistro": "2026-08-20T14:30:00+00:00",
    "esMejorOferta": true,
    "tipoCambioValor": null,
    "tipoCambioFecha": null
  }
]
```

`GET /api/v1/ofertas/{id}` retorna el mismo DTO para una oferta o `404 Not
Found` si no existe. En ambas rutas, `moneda=USD` presenta `monto / tipo de
cambio USD→CRC activo`; el monto persistido continúa en CRC. Una moneda distinta
de `CRC` o `USD`, o la ausencia de un tipo de cambio activo al solicitar USD,
devuelve `400 Bad Request`. Cuando la moneda es `USD`, la respuesta incluye
`tipoCambioValor` y `tipoCambioFecha` del tipo de cambio utilizado (HU-19);
con `CRC` ambos campos son nulos.

## Niveles de aprobación (HU-18)

### Crear

```http
POST /api/v1/niveles-aprobacion
Content-Type: application/json

{
  "nombre": "Compras Menores",
  "montoMinimo": 0,
  "montoMaximo": 1000000
}
```

Una solicitud válida devuelve `201 Created`, una cabecera `Location` con
`/api/v1/niveles-aprobacion/{id}` y el DTO siguiente:

```json
{ "id": 4, "nombre": "Compras Menores" }
```

Devuelve `409 Conflict` con título `Rango de aprobación en conflicto` cuando el
rango traslapa un nivel activo, incluido el intento de crear un segundo rango
abierto. Devuelve `400 Bad Request` con título `Nivel de aprobación inválido`
para nombre vacío, monto mínimo negativo o máximo menor o igual que el mínimo.
La creación aún no tiene operaciones complementarias: editar, listar y
desactivar no están expuestos por API.

### Resolver aprobador

```http
GET /api/v1/niveles-aprobacion/resolver?monto=7500000
```

Devuelve `200 OK` con el nivel activo que contiene el monto, eligiendo el de
`MontoMinimo` más alto entre los que lo contienen:

```json
{ "id": 2, "nombre": "Gerencial" }
```

Si ningún nivel activo contiene el monto devuelve `404 Not Found`. La resolución
consulta la tabla `NivelesAprobacion`; no existe lógica condicional fija por
rangos en el código.

## Tipos de cambio (HU-19)

### Guardar tipo de cambio

`POST /api/v1/tipos-cambio` registra un nuevo tipo de cambio activo USD→CRC y
desactiva automáticamente cualquier registro previamente activo: siempre
existe como máximo un tipo de cambio activo. Devuelve `201 Created`, una
cabecera `Location` con `/api/v1/tipos-cambio/{id}` y el DTO siguiente:

```http
POST /api/v1/tipos-cambio
Content-Type: application/json

{ "valor": 512, "fecha": "2026-08-22" }
```

```json
{
  "id": 2,
  "monedaOrigen": "USD",
  "monedaDestino": "CRC",
  "valor": 512,
  "fecha": "2026-08-22",
  "activo": true
}
```

Devuelve `400 Bad Request` con título `Tipo de cambio inválido` cuando el
valor no es mayor que cero.

### Consultar tipo de cambio activo

`GET /api/v1/tipos-cambio/activo` devuelve `200 OK` con el registro activo en
el mismo formato, o `404 Not Found` si no existe ninguno. La conversión de
ofertas en USD (`?moneda=USD`) consume este mismo registro administrado
localmente, sin llamadas a servicios externos. No existen todavía edición,
historial ni desactivación explícita por API.

## Contrato de errores (HU-26)

Toda respuesta 4xx y 5xx de la API usa `application/problem+json` con título,
estado, detalle seguro, `type`, `instance` (ruta solicitada) y las extensiones
`codigoError` e `correlacionId`; nunca incluye stack traces, rutas internas ni
secretos. Ejemplo:

```json
{
  "type": "https://httpstatuses.com/422",
  "title": "Oferta rechazada",
  "status": 422,
  "detail": "El monto de la oferta no puede superar el presupuesto de la licitacion.",
  "instance": "/api/v1/ofertas",
  "codigoError": "oferta_no_procesable",
  "correlacionId": "0HNO1EJRQ4B5D"
}
```

Mapeo general: errores de datos inválidos retornan `400`; recurso inexistente
`404`; conflictos de duplicidad o concurrencia `409`; reglas de negocio no
procesables `422`; y una excepción no prevista produce un `500` controlado con
código `error_interno`. El `correlacionId` corresponde al `TraceIdentifier` de
la solicitud y sirve para seguimiento en bitácoras del cliente.
