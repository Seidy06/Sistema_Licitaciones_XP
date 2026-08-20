# Módulo de licitaciones

## Alcance terminado

HU-10 implementa la creación de licitaciones con código único, título, presupuesto
positivo y fecha de cierre. HU-11 implementa la publicación de una licitación
desde estado `Borrador` hacia `Publicada`, con registro de la transición de
estado. HU-12 implementa la edición de licitaciones (con validación de presupuesto
frente a ofertas existentes y protección de campos tras el cierre) y el cierre
funcional basado en vencimiento de la fecha de cierre. HU-13 implementa el
listado y consulta de licitaciones con filtro por estado efectivo (incluyendo
cierre funcional). HU-16 completa el detalle con la mejor oferta, su porcentaje
de ahorro, clasificación y el mensaje aplicable cuando no existen ofertas.

## Reglas de negocio

### Crear licitación (HU-10)

- El código es obligatorio; se normaliza con `Trim().ToUpperInvariant()` y
  persiste como `CodigoNormalizado`.
- No pueden existir dos licitaciones activas con el mismo código normalizado.
- El título es obligatorio y admite hasta 250 caracteres.
- El presupuesto debe ser mayor que cero (validación en dominio, aplicación y
  restricción CHECK en PostgreSQL).
- La fecha de cierre se almacena como `timestamp with time zone`.
- Una licitación nueva inicia en estado `Borrador`.

### Publicar licitación (HU-11)

- Solo se puede publicar una licitación en estado `Borrador`.
- La fecha de cierre no puede estar vencida al momento de publicar.
- La publicación transiciona el estado a `Publicada`.
- Cada transición se registra en la tabla `licitacion_transiciones` con el
  estado anterior, el estado nuevo y la fecha de la transición.

### Editar licitación (HU-12)

- No se puede editar una licitación cerrada formalmente (`Estado == Cerrada`).
- No se puede editar una licitación cerrada funcionalmente
  (`Estado == Publicada` y `FechaCierre <= clock.UtcNow()`).
- Si se reduce el presupuesto por debajo del monto mínimo de oferta ya
  registrada, se rechaza la edición.
- Los campos código, título, presupuesto y fecha de cierre son editables
  mientras la licitación no esté cerrada (formal ni funcionalmente).
- Los campos nulos en la solicitud de edición conservan el valor actual
  (edición parcial).

### Cierre funcional (HU-12)

- `EstadoEfectivo(IClock)` retorna `Cerrada` cuando la licitación está en
  estado `Publicada` y `FechaCierre <= clock.UtcNow()`.
- El campo `Estado` en persistencia no cambia; el cierre es computado en
  tiempo de lectura.
- `EstaCerradaFormalmente()` retorna `true` cuando `Estado == Cerrada`.

### Listar y consultar licitaciones (HU-13 y HU-16)

- `GET /api/v1/licitaciones` retorna todas las licitaciones activas
  (`DeletedAt IS NULL`).
- El filtro por estado efectivo computa el estado real en el servicio:
  `Publicada` con `FechaCierre <= ahora` se muestra como `Cerrada`.
- `Borrador` con `FechaCierre` vencida se mantiene como `Borrador`
  (el cierre funcional solo aplica a `Publicada`).
- `GET /api/v1/licitaciones/{id}` retorna el detalle completo incluyendo
  código, título, presupuesto, fecha de cierre, mejor oferta y nivel de
  aprobación correspondiente.
- La mejor oferta incluye `Id`, `Monto`, `AhorroPorcentaje` y `Clasificacion`.
  El menor monto gana y los empates se resuelven por la `FechaRegistro` más
  temprana.
- Si no existen ofertas para una licitación, `MejorOferta` y
  `NivelAprobacion` son `null`, y `MensajeMejorOferta` contiene
  `Sin ofertas válidas`.

## Componentes

| Capa | Componentes principales |
| --- | --- |
| Domain | `Licitacion` (entidad con `Crear`, `Publicar`, `Editar`, `EstaVencida`, `EstaCerradaFormalmente`, `EstadoEfectivo`), `LicitacionTransicion`, `EstadoLicitacion`, `EstadoLicitacionCatalogo`; `CalculadoraMejorOferta` y `ResultadoMejorOferta` para selección y clasificación. |
| Application | `CrearLicitacionService`, `EditarLicitacionService`, `ConsultarLicitacionService`, `ILicitacionRepository`, `ILicitacionConsultaRepository`, `LicitacionDto` (con `FromEntity`), `ConsultarLicitacionesRequest`, `LicitacionConsultaDto`, `LicitacionDetalleDto`, `LicitacionMejorOfertaDto`, `LicitacionNivelAprobacionDto`, `PaginaLicitaciones`, `LicitacionNoEncontradaException`. |
| Infrastructure | `LicitacionRepository`, `LicitacionConsultaRepository`, configuraciones EF Core, migraciones `ImplementCreateTenderHu10` e `ImplementPublishTenderHu11`. |
| API | Crear licitación bajo `POST /api/v1/licitaciones`. Listar y consultar bajo `GET /api/v1/licitaciones` y `GET /api/v1/licitaciones/{id}`. |
| Web | Formulario de creación en MVC. |

## Máquina de estados

```
Borrador ──Publicar()──► Publicada ──vencimiento──► Cerrada (funcional)
                       │
                       └──cierre formal──► Cerrada
```

Las transiciones hacia `Adjudicada` y `Cancelada` se implementarán en
historias posteriores.

## Persistencia

La tabla `Licitaciones` almacena la entidad con código único parcial filtrado
por `DeletedAt IS NULL`. La tabla `licitacion_transiciones` almacena el
historial de cambios de estado con FK cascada hacia `Licitaciones`.
