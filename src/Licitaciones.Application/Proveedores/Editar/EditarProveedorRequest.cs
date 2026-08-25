namespace Licitaciones.Application.Proveedores.Editar;

/// <summary>
/// Datos para actualizar un proveedor existente con control de concurrencia.
/// </summary>
public sealed record EditarProveedorRequest(string Nombre, uint Version);
