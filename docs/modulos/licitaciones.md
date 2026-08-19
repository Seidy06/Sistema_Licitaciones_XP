# Módulo de licitaciones

## Alcance terminado

HU-10 implementa la creación de licitaciones con código único, título, presupuesto
positivo y fecha de cierre. HU-11 implementa la publicación de una licitación
desde estado `Borrador` hacia `Publicada`, con registro de la transición de
estado.

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

## Componentes

| Capa | Componentes principales |
| --- | --- |
| Domain | `Licitacion` (entidad con `Crear`, `Publicar`, `EstaVencida`), `LicitacionTransicion`, `EstadoLicitacion`, `EstadoLicitacionCatalogo`. |
| Application | `CrearLicitacionService`, `ILicitacionRepository`, `LicitacionDto`. |
| Infrastructure | `LicitacionRepository`, configuraciones EF Core, migraciones `ImplementCreateTenderHu10` e `ImplementPublishTenderHu11`. |
| API | Crear licitación bajo `POST /api/v1/licitaciones`. |
| Web | Formulario de creación en MVC. |

## Máquina de estados

```
Borrador ──Publicar()──► Publicada
```

Las transiciones futuras (Cerrada, Adjudicada, Cancelada) se implementarán en
historias posteriores (HU-12 y siguientes).

## Persistencia

La tabla `Licitaciones` almacena la entidad con código único parcial filtrado
por `DeletedAt IS NULL`. La tabla `licitacion_transiciones` almacena el
historial de cambios de estado con FK cascada hacia `Licitaciones`.
