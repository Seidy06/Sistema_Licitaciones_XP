namespace Licitaciones.Application.Proveedores.Crear;

public sealed class ProveedorDuplicadoException : Exception
{
    public ProveedorDuplicadoException(string nombre)
        : base($"Ya existe un proveedor con el nombre '{nombre}'.")
    {
    }
}
