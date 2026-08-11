# Integración de módulos

## Registro de proveedores

El caso de uso está compartido por la interfaz MVC y la API REST:

```text
MVC / API
    ↓
CrearProveedorService
    ↓
Proveedor + normalización de Domain
    ↓
IProveedorRepository
    ↓
Entity Framework Core
    ↓
PostgreSQL
```

### Registro de dependencias

La Web registra `CrearProveedorService`, `IProveedorRepository` y
`LicitacionesDbContext` con alcance por solicitud. La cadena `Licitaciones` se
obtiene desde configuración y apunta al PostgreSQL local durante desarrollo.

### Contratos entre capas

- MVC transforma `CrearProveedorViewModel` en `CrearProveedorRequest`.
- API transforma su contrato HTTP en el mismo request de Application.
- Application retorna `ProveedorDto` y no expone tipos de EF Core.
- Infrastructure implementa `IProveedorRepository`.
- `ProveedorDuplicadoException` se traduce a error junto al campo en MVC y a
  `409 Conflict` en la API.

Esta separación permite cambiar la presentación sin modificar las reglas ni la
persistencia.
