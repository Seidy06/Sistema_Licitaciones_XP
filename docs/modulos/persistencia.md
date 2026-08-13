# Persistencia

La persistencia utiliza Entity Framework Core 9 con el proveedor Npgsql para
PostgreSQL. `LicitacionesDbContext` aplica los mapeos Fluent API del ensamblado
de infraestructura y centraliza las marcas `CreatedAt` y `UpdatedAt` mediante
el reloj inyectable `IClock`.

La cadena de conexión no se almacena en el repositorio. En ejecución se define
con `ConnectionStrings__Licitaciones`; para herramientas de diseño de EF Core
se usa `LICITACIONES_DESIGN_CONNECTION_STRING`.

Las migraciones se encuentran en
`src/Licitaciones.Infrastructure/Persistence/Migrations`. La migración
`CompleteInitialDomain` agrega licitaciones, estados, ofertas, niveles de
aprobación y tipos de cambio, junto con sus semillas y restricciones.
