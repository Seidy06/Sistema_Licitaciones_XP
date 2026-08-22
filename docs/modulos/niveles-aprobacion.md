# Módulo de niveles de aprobación

HU-18 agrega la administración inicial de los niveles de aprobación como tabla
parametrizable: la creación por API y la resolución del aprobador consultando
la tabla, sin cadenas de `if/else` en el código. El alcance actual cubre crear
y resolver; las operaciones de editar, listar y desactivar del enunciado de la
historia aún no están expuestas y no se documentan como existentes.

## Caso de uso implementado

`AdministrarNivelesAprobacionService.CrearAsync(...)` coordina la creación:

1. Verifica mediante `INivelAprobacionRepository.ExisteTraslapeActivoAsync(...)`
   que ningún nivel activo traslape el rango solicitado; si existe traslape
   lanza `NivelAprobacionConflictoException`.
2. Delega los invariantes del rango a `NivelAprobacion.Crear(...)` en Domain:
   nombre obligatorio, monto mínimo no negativo y monto máximo mayor que el
   mínimo.
3. Persiste el nivel activo y retorna un `LicitacionNivelAprobacionDto` con
   identificador y nombre.

La comprobación previa en Application mejora el mensaje; ante dos creaciones
concurrentes es la restricción de exclusión de PostgreSQL la que rechaza el
segundo registro, y el repositorio traduce esa violación a
`NivelAprobacionConflictoException`.

## Resolución del aprobador

`ResolverNivelAprobacionService.ResolverAsync(monto)` consulta la tabla a
través de `ILicitacionConsultaRepository.ObtenerNivelAprobacionAsync(...)`.
La consulta selecciona entre los niveles activos aquellos cuyo rango contiene
el monto (`MontoMinimo <= monto` y `MontoMaximo` nulo o `monto <= MontoMaximo`)
y retorna el de `MontoMinimo` más alto. Si ningún nivel activo contiene el
monto, retorna `null` y la API responde `404`. La misma consulta alimenta el
campo `nivelAprobacion` del detalle de licitación de HU-13/HU-16.

## Componentes

| Capa | Componentes y responsabilidad |
| --- | --- |
| Domain | `NivelAprobacion.Crear(...)` valida nombre, monto mínimo y relación máximo/mínimo; la entidad nace activa y auditable. |
| Application | `AdministrarNivelesAprobacionService`, `INivelAprobacionRepository`, `NivelAprobacionConflictoException` y `ResolverNivelAprobacionService`. El DTO `LicitacionNivelAprobacionDto` permanece en el contrato de consulta de licitaciones reutilizado por HU-13 y HU-16. |
| Infrastructure | `NivelAprobacionRepository` verifica traslapes entre activos, persiste y traduce la violación de `EX_NivelesAprobacion_SinTraslape`; `LicitacionConsultaRepository` resuelve el aprobador filtrando por `Activo`. |
| API | `NivelesAprobacionController` adapta el contrato HTTP y convierte `NivelAprobacionConflictoException` en `409` y `DomainException` en `400`; las reglas permanecen fuera del controlador. |

## Persistencia

PostgreSQL conserva los montos como `numeric(18,2)`, aplica los CHECK
`CK_NivelesAprobacion_Minimo` y `CK_NivelesAprobacion_Rango`, y garantiza la
no superposición de rangos activos con la restricción de exclusión
`EX_NivelesAprobacion_SinTraslape` sobre `numrange("MontoMinimo",
"MontoMaximo", '[)') WITH &&`, limitada a `WHERE ("Activo")`. Los identificadores
se generan con la secuencia `NivelesAprobacion_Id_seq` iniciada en 4, después
de las semillas Operativo, Gerencial y Directivo.

La migración `AdministrarNivelesAprobacionHu18` agregó la columna `Activo`
(por defecto `true`), recreó la restricción de exclusión con ese filtro y
creó la secuencia; así un nivel desactivado en el futuro podrá coexistir con
rangos nuevos sin violar la restricción.

## API

`POST /api/v1/niveles-aprobacion` recibe `nombre`, `montoMinimo` y
`montoMaximo` opcional. Devuelve `201 Created` con el DTO y cabecera `Location`;
devuelve `409 Conflict` cuando el rango traslapa otro nivel activo o ya existe
un rango abierto, y `400 Bad Request` para datos inválidos controlados por el
dominio o el contrato.

`GET /api/v1/niveles-aprobacion/resolver?monto={monto}` devuelve `200 OK` con
el nivel activo que contiene el monto, o `404 Not Found` si ninguno lo
contiene. No existe todavía edición, listado ni desactivación por API.

## Pruebas

HU-18 agrega cinco pruebas de integración sobre PostgreSQL real:

- Dos pruebas de persistencia comprueban que la restricción de exclusión
  rechaza un segundo rango traslapado y un segundo rango abierto con el error
  `23P01`, dentro de una transacción revertida.
- Tres pruebas HTTP verifican que el traslape responde `409` sin persistir el
  segundo nivel, que un segundo rango abierto se rechaza con `409` y que la
  resolución consulta realmente la tabla insertando un nivel especial,
  resolviendo un monto dentro de su rango y restaurando el catálogo sembrado.
