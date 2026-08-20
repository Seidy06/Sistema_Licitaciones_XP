# Módulo de ofertas

HU-14 implementa el registro económico de un proveedor en una licitación por
medio de la API REST. HU-15 agrega respuestas específicas para ofertas
duplicadas, vencidas o superiores al presupuesto y protege las ofertas
registradas contra edición y eliminación. HU-16 calcula la mejor oferta y su
clasificación de ahorro para el detalle de una licitación. El alcance actual no
incluye vistas MVC, listado de ofertas ni un registro persistente separado de
intentos rechazados.

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

## Mejor oferta y ahorro (HU-16)

`CalculadoraMejorOferta.Calcular(...)` es un servicio estático y puro de
Domain. Recibe el presupuesto y las ofertas válidas de una licitación, sin
acceder a controladores ni persistencia:

1. Ordena por `Monto` ascendente.
2. Si hay empate, ordena por `FechaRegistro` ascendente.
3. Calcula `AhorroPorcentaje = (Presupuesto - Monto) / Presupuesto * 100`.
4. Clasifica un ahorro mayor o igual a 10 % como `Oferta conveniente`, uno
   mayor que 0 % y menor que 10 % como `Oferta aceptable`, y un monto igual al
   presupuesto como `Oferta válida sin ahorro`.

Si no recibe ofertas, retorna `null`; Application lo presenta mediante
`MensajeMejorOferta` con el texto `Sin ofertas válidas`.

## Componentes

| Capa | Componentes y responsabilidad |
| --- | --- |
| Domain | `Oferta.Crear(...)` protege los invariantes propios de la entidad. `CalculadoraMejorOferta` selecciona, desempata y clasifica; `ResultadoMejorOferta` devuelve identificador, monto, porcentaje y clasificación. |
| Application | `CrearOfertaService`, `CrearOfertaRequest`, `IOfertaRepository`, `OfertaDuplicadaException` y `OfertaDto` ejecutan el registro. `ProtegerOfertaService`, `IProteccionOfertaRepository` y `OfertaErrorCodes` expresan la inmutabilidad y los rechazos no procesables. `ConsultarLicitacionService` aplica el cálculo al detalle. |
| Infrastructure | `OfertaRepository` consulta licitación/proveedor, detecta duplicidad, persiste, obtiene la licitación asociada a una oferta y traduce la violación del índice compuesto esperado. `LicitacionConsultaRepository` obtiene las ofertas válidas para el cálculo. |
| API | `OfertasController` adapta el contrato HTTP y convierte rechazos de negocio en `400`, `409` o `422`; sus rutas `PUT` y `DELETE` protegen la evidencia en vez de modificarla. |

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
la oferta duplicada, `422 Unprocessable Entity` para vencimiento o exceso de
presupuesto, y `400 Bad Request` para los demás rechazos controlados.

`PUT /api/v1/ofertas/{id}` y `DELETE /api/v1/ofertas/{id}` no alteran ni eliminan
la oferta. Devuelven `422 Unprocessable Entity`; cuando pertenece a una
licitación cerrada, el detalle indica explícitamente que no puede editarse ni
eliminarse. La oferta permanece en PostgreSQL con licitación, proveedor y monto
inalterados como evidencia histórica.

## Pruebas

HU-14 cuenta con pruebas unitarias del servicio, pruebas HTTP mediante
`WebApplicationFactory` y pruebas de persistencia sobre PostgreSQL real. HU-15
agrega cinco pruebas HTTP integradas para códigos y mensajes de duplicidad,
vencimiento y presupuesto, además de edición/eliminación de ofertas de una
licitación cerrada con verificación posterior de la evidencia persistida.
HU-16 agrega cinco pruebas de Application y cinco pruebas HTTP integradas para
el monto mínimo, el desempate por fecha, el caso sin ofertas y los tres rangos
de clasificación.
