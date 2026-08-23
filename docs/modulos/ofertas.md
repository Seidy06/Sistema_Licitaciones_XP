# Módulo de ofertas

HU-14 implementa el registro económico de un proveedor en una licitación por
medio de la API REST. HU-15 agrega respuestas específicas para ofertas
duplicadas, vencidas o superiores al presupuesto y protege las ofertas
registradas contra edición y eliminación. HU-16 calcula la mejor oferta y su
clasificación de ahorro para el detalle de una licitación. HU-17 agrega el
listado por licitación y la consulta por identificador mediante la API, con
presentación CRC o USD según el tipo de cambio activo. El alcance actual no
incluye vistas MVC, paginación, filtro ni ordenamiento del listado, ni un
registro persistente separado de intentos rechazados.

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

## Listado y consulta de ofertas (HU-17)

`ConsultarOfertaService` expone dos operaciones de lectura:

- `ListarAsync(licitacionId, moneda)`: retorna las ofertas registradas para
  una licitación.
- `ObtenerAsync(id, moneda)`: retorna una oferta por identificador o `null`
  si no existe; para calcular el indicador de mejor oferta consulta el
  listado completo de su licitación.

Cada elemento es un `OfertaConsultaDto` con identificador, nombre del
proveedor, monto, moneda, fecha de registro e indicador `EsMejorOferta`; al
solicitar `USD` incluye además `tipoCambioValor` y `tipoCambioFecha` del
tipo de cambio utilizado (HU-19), nulos con `CRC`. El
indicador se calcula con la misma regla de HU-16: menor monto y, en caso de
empate, la `FechaRegistro` más temprana. La moneda acepta `CRC` (valor
persistido, por defecto) o `USD`; en USD el monto se divide entre el tipo de
cambio activo obtenido del repositorio y se rechaza la consulta si no existe
un tipo de cambio activo. La conversión ocurre solo en presentación: el monto
almacenado permanece en CRC.

## Componentes

| Capa | Componentes y responsabilidad |
| --- | --- |
| Domain | `Oferta.Crear(...)` protege los invariantes propios de la entidad. `CalculadoraMejorOferta` selecciona, desempata y clasifica; `ResultadoMejorOferta` devuelve identificador, monto, porcentaje y clasificación. |
| Application | `CrearOfertaService`, `CrearOfertaRequest`, `IOfertaRepository`, `OfertaDuplicadaException` y `OfertaDto` ejecutan el registro. `ProtegerOfertaService`, `IProteccionOfertaRepository` y `OfertaErrorCodes` expresan la inmutabilidad y los rechazos no procesables. `ConsultarLicitacionService` aplica el cálculo al detalle. `ConsultarOfertaService`, `IOfertaConsultaRepository` y `OfertaConsultaDto` implementan el listado y la consulta de HU-17. |
| Infrastructure | `OfertaRepository` consulta licitación/proveedor, detecta duplicidad, persiste, obtiene la licitación asociada a una oferta y traduce la violación del índice compuesto esperado; como `IOfertaConsultaRepository` obtiene las ofertas con su proveedor. El tipo de cambio activo lo provee `TipoCambioRepository` mediante `ITipoCambioRepository` (HU-19). `LicitacionConsultaRepository` obtiene las ofertas válidas para el cálculo de la mejor oferta. |
| API | `OfertasController` adapta el contrato HTTP y convierte rechazos de negocio en `400`, `409` o `422`; sus rutas `PUT` y `DELETE` protegen la evidencia en vez de modificarla. Las rutas `GET` atienden el listado y la consulta de HU-17. |

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

`GET /api/v1/ofertas?licitacionId={guid}&moneda=CRC|USD` lista las ofertas de
una licitación y devuelve `200 OK`; `GET /api/v1/ofertas/{id}?moneda=CRC|USD`
consulta una oferta y devuelve `200 OK`, `404 Not Found` si no existe o
`400 Bad Request` si la moneda solicitada no es CRC ni USD. Ambos aceptan
`moneda` con valor por defecto `CRC`. El listado aún no expone paginación,
filtro ni ordenamiento.

## Pruebas

HU-14 cuenta con pruebas unitarias del servicio, pruebas HTTP mediante
`WebApplicationFactory` y pruebas de persistencia sobre PostgreSQL real. HU-15
agrega cinco pruebas HTTP integradas para códigos y mensajes de duplicidad,
vencimiento y presupuesto, además de edición/eliminación de ofertas de una
licitación cerrada con verificación posterior de la evidencia persistida.
HU-16 agrega cinco pruebas de Application y cinco pruebas HTTP integradas para
el monto mínimo, el desempate por fecha, el caso sin ofertas y los tres rangos
de clasificación. HU-17 agrega dos pruebas HTTP integradas: el listado
comprueba proveedor, monto CRC, fecha de registro e indicador de mejor oferta,
y el detalle solicita USD y comprueba la conversión con el tipo de cambio
activo sin alterar el monto persistido.

## Correcciones de cierre de la Iteracion 2

El listado HU-17 responde ahora con `items`, `total`, `pagina` y
`tamanoPagina`. Ademas de `licitacionId` y `moneda`, acepta filtro `proveedor`,
orden por `monto`, `proveedor` o `fechaRegistro`, direccion `descendente`,
`pagina` y `tamanoPagina` (maximo 100). La seleccion de la mejor oferta se
calcula sobre todas las ofertas de la licitacion antes de filtrar o paginar.

La cobertura HTTP integrada consta de tres casos: contrato CRC, detalle USD y
filtro/orden/paginacion. Todos reutilizan la unica fixture PostgreSQL compartida.
