# Módulo de persistencia

La persistencia usa Entity Framework Core 9.0 con Npgsql 9.0 y PostgreSQL. `LicitacionesDbContext` descubre las configuraciones Fluent API del ensamblado y sobrescribe `SaveChanges`/`SaveChangesAsync` para asignar auditoría mediante `IClock`.

`ProveedorRepository` implementa las interfaces de registro, consulta y baja. Realiza consultas sin seguimiento cuando corresponde, aplica filtro, ordenamiento y `Skip/Take` en PostgreSQL, y traduce:

- la violación `23505` de `UX_Proveedores_NombreNormalizado` a conflicto de duplicidad;
- `DbUpdateConcurrencyException` a `ProveedorConcurrenciaException` durante la edición.

La configuración de proveedores usa `DeletedAt` como baja lógica, filtro global para activos, índice único parcial y `xmin` como token de fila. No hay eliminación física en el caso de uso implementado.

Las migraciones están en `src/Licitaciones.Infrastructure/Persistence/Migrations`. Web ejecuta `Database.MigrateAsync()` al arrancar; API no lo hace. En ejecución se usa `ConnectionStrings__Licitaciones`; la fábrica de diseño admite `LICITACIONES_DESIGN_CONNECTION_STRING`.

El contexto también contiene el esquema base de licitaciones, ofertas, estados, niveles de aprobación y tipos de cambio. Su persistencia existe, pero no equivale a casos de uso terminados para esos módulos.
