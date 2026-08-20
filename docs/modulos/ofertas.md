# Módulo de ofertas

HU-14 implementa el registro económico de un proveedor en una licitación por
medio de la API REST. El alcance actual no incluye vistas MVC, listado de
ofertas, auditoría de intentos rechazados ni clasificación de ahorro.

## Caso de uso implementado

`CrearOfertaService.CrearAsync(...)` coordina el registro y conserva el orden de
validación definido por la historia:

1. La licitación debe existir y estar en estado `Publicada`.
2. La fecha actual, obtenida mediante `IClock`, debe ser anterior a
   `FechaCierre`.
3. El proveedor no debe tener otra oferta para la misma licitación.
4. El monto no puede superar el presupuesto; un monto igual es válido.
5. El monto debe ser mayor que cero.
6. El proveedor debe existir y estar activo.

Las reglas se ejecutan en Application y Domain, no en el controlador. La
entidad `Oferta` valida identificadores, monto positivo y registra
`FechaRegistro` con el reloj inyectado.

## Componentes

| Capa | Componentes y responsabilidad |
| --- | --- |
| Domain | `Oferta.Crear(...)` protege los invariantes propios de la entidad. |
| Application | `CrearOfertaService`, `CrearOfertaRequest`, `IOfertaRepository`, `OfertaDuplicadaException` y `OfertaDto` ejecutan y expresan el caso de uso. |
| Infrastructure | `OfertaRepository` consulta licitación/proveedor, detecta duplicidad, persiste y traduce la violación del índice compuesto esperado. |
| API | `OfertasController` adapta el contrato HTTP y convierte rechazos de negocio en `400` o `409`. |

## Persistencia y concurrencia

PostgreSQL conserva el monto como `numeric(18,2)`, aplica el CHECK
`CK_Ofertas_Monto_Positivo`, FKs restrictivas hacia licitación y proveedor, y
el índice único `IX_Ofertas_LicitacionId_ProveedorId`. La comprobación previa
mejora el mensaje normal, mientras el índice resuelve correctamente dos
registros concurrentes. Solo la violación de ese índice se traduce a
`OfertaDuplicadaException`.

## API

`POST /api/v1/ofertas` recibe `licitacionId`, `proveedorId` y `monto`. Devuelve
`201 Created` con el DTO y una cabecera `Location`; devuelve `409 Conflict` para
la oferta duplicada y `400 Bad Request` para los demás rechazos controlados.

## Pruebas

HU-14 cuenta con pruebas unitarias del servicio, pruebas HTTP mediante
`WebApplicationFactory` y pruebas de persistencia sobre PostgreSQL real. Se
cubren estado, cierre funcional, duplicidad, presupuesto, monto positivo, FKs,
CHECK e índice único compuesto.
