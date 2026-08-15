# HU-08: Borrado lógico de proveedores

## Alcance

HU-08 permite dar de baja un proveedor sin eliminar físicamente su fila. La
operación establece `DeletedAt`, conserva el nombre, los datos de auditoría y
las relaciones históricas, y oculta al proveedor en las operaciones ordinarias.

## Comportamiento del dominio

`Proveedor.DarDeBaja(instante)` asigna la fecha de eliminación sólo cuando el
proveedor sigue activo. `EstaEliminado` indica si `DeletedAt` tiene valor. El
caso de uso `DarBajaProveedorService` obtiene el instante mediante `IClock` para
mantener el comportamiento determinista en pruebas.

Si el proveedor activo no existe, Application produce
`ProveedorNoEncontradoException`. No se ejecuta una eliminación física.

## Persistencia

La migración `AddProveedorSoftDelete` agrega la columna nullable `DeletedAt` a
`Proveedores`. Entity Framework Core configura:

```csharp
builder.HasQueryFilter(proveedor => proveedor.DeletedAt == null);
```

Por ello, listados, detalles, edición y baja sólo encuentran proveedores
activos. Las consultas históricas explícitas pueden usar `IgnoreQueryFilters()`
para recuperar la fila preservada.

El índice `UX_Proveedores_NombreNormalizado` es único únicamente para filas
activas mediante el filtro PostgreSQL `"DeletedAt" IS NULL`. Esto permite
registrar nuevamente un nombre que sólo pertenece a un proveedor dado de baja,
sin perder el registro histórico anterior.

## API REST

`DELETE /api/v1/proveedores/{id}`

| Estado | Condición |
| --- | --- |
| `204 No Content` | El proveedor activo fue dado de baja. |
| `404 Not Found` | No existe un proveedor activo con ese identificador. |

La respuesta 204 no contiene cuerpo. Los endpoints ordinarios de listado y
detalle dejan de mostrar el proveedor inmediatamente después de la baja.

## MVC

1. `GET /Proveedores/Delete/{id}` presenta el nombre del proveedor y solicita
   confirmación explícita.
2. El usuario puede cancelar y regresar al detalle sin producir cambios.
3. `POST /Proveedores/DeleteConfirmed/{id}` ejecuta la baja lógica y redirige
   al listado.
4. Un identificador inexistente o ya eliminado produce `NotFound`.

## Evidencia TDD

### ROJO — `a74b9cd`

- Se exigió que `DeletedAt` se estableciera usando un reloj controlado.
- Se probaron el filtro global, la ausencia en listados activos y la
  recuperación histórica mediante `IgnoreQueryFilters()`.
- Se verificó que la fila y sus datos históricos no fueran eliminados.
- Se fijaron los contratos DELETE 204/404 y la confirmación MVC previa.

### VERDE

- `cc43bd2` agregó `DeletedAt`, el filtro global, el índice único parcial y la
  migración de PostgreSQL.
- `aed1feb` implementó el comportamiento de dominio, el caso de uso, el
  repositorio, DELETE en API y la confirmación en MVC.
- No se incorporó ninguna eliminación física de proveedores.

## Verificación

```powershell
dotnet build Licitaciones.sln --configuration Release
dotnet test Licitaciones.sln --configuration Release
git diff --check
git status
```
