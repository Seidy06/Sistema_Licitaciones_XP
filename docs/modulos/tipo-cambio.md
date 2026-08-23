# Módulo de tipos de cambio

HU-19 agrega la administración del tipo de cambio USD→CRC y la conversión de
presentación CRC↔USD para las ofertas. Los montos oficiales se almacenan
siempre en colones: la conversión es únicamente un cálculo de presentación que
divide entre el valor del tipo de cambio activo y nunca modifica el valor
persistido. El alcance actual es por API; la alternancia de moneda desde la
interfaz web aún no existe y no se documenta como implementada.

La conversión funciona sin conexión a Internet: consulta exclusivamente el
registro administrado localmente en la tabla `TiposCambio`; no hay dependencia
de servicios ni APIs externas.

## Administración del tipo de cambio

`AdministrarTipoCambioService.GuardarAsync(valor, fecha)` coordina el caso de
uso:

1. Delega los invariantes a `TipoCambio.Crear(...)` en Domain: el valor debe
   ser mayor que cero (`DomainException` en caso contrario) y el registro nace
   activo con par fijo USD→CRC y la fecha indicada.
2. Delega a `ITipoCambioRepository.ReemplazarActivoAsync(...)`, que desactiva
   todos los registros activos previos (`Desactivar()`) y persiste el nuevo.
3. Retorna un `TipoCambioDto` con identificador, par, valor, fecha y estado.

La regla «solo un tipo de cambio activo» queda reforzada en base de datos por
el índice único parcial `UX_TiposCambio_Activo` (`WHERE "Activo"`), que impide
dos filas activas incluso ante escrituras concurrentes.

## Conversión de presentación en ofertas

`ConsultarOfertaService.ConvertirAsync(...)` atiende el parámetro `moneda` de
las rutas `GET /api/v1/ofertas` y `GET /api/v1/ofertas/{id}`:

1. Normaliza el valor (`trim` y mayúsculas); acepta únicamente `CRC` o `USD`
   y lanza `DomainException` para cualquier otro valor.
2. Cuando se solicita `USD`, obtiene el registro activo con
   `ITipoCambioRepository.ObtenerActivoAsync(...)`; si no existe o su valor no
   es positivo, lanza `DomainException`.
3. Presenta cada monto como `monto / tipoCambio.Valor` junto con la moneda
   solicitada, e incluye `tipoCambioValor` y `tipoCambioFecha` del registro
   utilizado. Con `CRC` ambos campos son nulos y el monto no se transforma.
4. El mejor-oferta y su desempate se calculan sobre los montos originales en
   CRC, no sobre los convertidos.

## Componentes

| Capa | Componentes y responsabilidad |
| --- | --- |
| Domain | `TipoCambio.Crear(...)` valida el valor positivo y define el par fijo USD→CRC mediante las constantes `MonedaOrigenPredeterminada` y `MonedaDestinoPredeterminada`; `Desactivar()` marca el registro inactivo. |
| Application | `AdministrarTipoCambioService`, `ITipoCambioRepository` y `TipoCambioDto` para la administración; la conversión vive en `ConsultarOfertaService` (caso de uso de HU-17) consumiendo `ITipoCambioRepository`. Las reglas permanecen fuera de los controladores. |
| Infrastructure | `TipoCambioRepository` obtiene el registro activo filtrando por `Activo` y el par de monedas, y reemplaza el activo desactivando los previos; la consulta de ofertas usa el mismo repositorio. |
| API | `TiposCambioController` adapta el contrato HTTP y convierte `DomainException` en `400`; `OfertasController` expone `moneda` como parámetro de consulta. |

## Persistencia

La tabla `TiposCambio` existe desde la semilla inicial y HU-19 no agregó
migraciones: la conserva con `Valor` como `numeric(18,6)` bajo el CHECK
`CK_TiposCambio_Valor_Positivo`, fechas `date`/`timestamp with time zone` y el
índice único parcial `UX_TiposCambio_Activo` sobre el registro activo. La
semilla es el registro `Id = 1`, USD→CRC con valor `500` y fecha
`2026-01-01`, activo; las pruebas de integración restauran esta base después
de cada escenario.

## API

`POST /api/v1/tipos-cambio` recibe `valor` y `fecha`. Devuelve `201 Created`
con el DTO y cabecera `Location` hacia `/api/v1/tipos-cambio/{id}`; devuelve
`400 Bad Request` con título `Tipo de cambio inválido` cuando el dominio
rechaza el valor.

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

`GET /api/v1/tipos-cambio/activo` devuelve `200 OK` con el registro activo en
el mismo formato, o `404 Not Found` si no existe ninguno.

No existen todavía edición, historial de tasas ni desactivación explícita por
API; el reemplazo del activo es la única operación de escritura.

## Pruebas

HU-19 agrega cuatro pruebas de integración con PostgreSQL real, todas con el
trait `HU-19` en `AdministrarTipoCambioHttpTests`:

- Guardar un nuevo tipo de cambio desactiva el previo y deja un único
  activo, verificando contra la base que el registro semilla quedó inactivo.
- Tras guardar una tasa nueva, `GET /api/v1/ofertas/{id}?moneda=USD` divide
  el monto entre el nuevo valor sin alterar el monto persistido en CRC.
- La vista en USD incluye el valor y la fecha del tipo de cambio utilizado
  (`tipoCambioValor` y `tipoCambioFecha`).
- Bloqueando toda llamada HTTP saliente en la fábrica de pruebas, tanto la
  consulta del activo como la conversión siguen funcionando con el registro
  local.
