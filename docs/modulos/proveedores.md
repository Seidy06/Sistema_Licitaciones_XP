# Módulo de proveedores

## Alcance implementado

HU-01 permite registrar proveedores desde MVC y API REST. El nombre se conserva
en una forma legible y también en `NombreNormalizado`, utilizado para comparar
sin distinguir mayúsculas, minúsculas, espacios redundantes ni variantes
Unicode equivalentes.

## Reglas

- El nombre es obligatorio y admite letras, números, espacios, punto, coma y
  paréntesis.
- Los espacios laterales se eliminan y los repetidos se reducen a uno.
- La normalización usa Unicode Form C y mayúsculas invariantes.
- Dos nombres con el mismo valor normalizado representan un duplicado.
- `Nombre` y `NombreNormalizado` tienen un máximo persistido de 200 caracteres.

Las reglas residen en Domain y son ejecutadas por `CrearProveedorService`; los
controladores no vuelven a implementarlas.

## Componentes

| Capa | Responsabilidad |
| --- | --- |
| Domain | Validar, normalizar y crear `Proveedor`. |
| Application | Detectar duplicidad y coordinar el caso de uso. |
| Infrastructure | Consultar y guardar con EF Core/PostgreSQL. |
| API | Exponer `POST /api/v1/proveedores`. |
| Web | Exponer `GET/POST /Proveedores/Create`. |

## Flujo MVC

1. `GET /Proveedores/Create` presenta el formulario.
2. La validación visual muestra el error junto a `Nombre`.
3. Un POST válido invoca `CrearProveedorService.CrearAsync`.
4. El servicio normaliza, comprueba duplicidad y persiste mediante el
   repositorio.
5. La vista informa el éxito o muestra el error asociado con el campo.

Casos verificados:

- `Empresa Central`: registro exitoso.
- `empresa central`: `Ya existe un proveedor con ese nombre.`
- `Empresa @ Central`: mensaje de caracteres no permitidos junto al campo.
