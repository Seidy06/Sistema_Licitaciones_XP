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
| `409 Conflict` | Ya existe el nombre normalizado, incluida una inserción concurrente rechazada por `UX_Proveedores_NombreNormalizado`. | `ProblemDetails` con título `Proveedor duplicado`. |

La API y MVC usan el mismo `CrearProveedorService`; por eso no duplican reglas
de negocio en sus controladores. La normalización Unicode Form C reside en
Domain. Infrastructure traduce específicamente la violación `23505` del índice
único a `ProveedorDuplicadoException`, por lo que una carrera esperada responde
`409 Conflict` y nunca `500 Internal Server Error`.
