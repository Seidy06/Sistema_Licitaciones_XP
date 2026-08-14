namespace Licitaciones.Application.Proveedores.Editar;

public sealed class ProveedorDuplicadoException : Exception
{
    public ProveedorDuplicadoException(string nombre)
        : base($"Ya existe otro proveedor con el nombre '{nombre}'.")
    {
    }
}
