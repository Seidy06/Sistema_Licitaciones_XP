namespace Licitaciones.Application.Proveedores.Editar;

public sealed class ProveedorNoEncontradoException : Exception
{
    public ProveedorNoEncontradoException(Guid id)
        : base($"No se encontró el proveedor '{id}'.")
    {
    }
}
