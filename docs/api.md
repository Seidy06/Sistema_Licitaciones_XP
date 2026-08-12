# API REST

## Registrar proveedor

`POST /api/v1/proveedores`

### Solicitud

```json
{
  "nombre": "Empresa Central"
}
```

### Respuesta exitosa

Estado: `201 Created`. La cabecera `Location` apunta a
`/api/v1/proveedores/{id}`.

```json
{
  "id": "7d9413f2-2bde-4bc9-af45-39a66f8fcce5",
  "nombre": "Empresa Central",
  "nombreNormalizado": "EMPRESA CENTRAL",
  "createdAt": "2026-08-11T18:00:00+00:00",
  "updatedAt": "2026-08-11T18:00:00+00:00",
  "version": 0
}
```

Los identificadores y fechas del ejemplo son ilustrativos.

### Errores

| Estado | Condición | Respuesta |
| --- | --- | --- |
| `400 Bad Request` | Nombre vacío o con caracteres no permitidos. | `ProblemDetails` con detalle comprensible. |
| `409 Conflict` | Ya existe el nombre normalizado. | `ProblemDetails` con título `Proveedor duplicado`. |

La API y MVC usan el mismo `CrearProveedorService`; por eso no duplican reglas
de negocio en sus controladores.
