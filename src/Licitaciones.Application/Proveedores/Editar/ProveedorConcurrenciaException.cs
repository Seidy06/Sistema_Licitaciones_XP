namespace Licitaciones.Application.Proveedores.Editar;

public sealed class ProveedorConcurrenciaException : Exception
{
    public ProveedorConcurrenciaException(Guid id)
        : base($"El proveedor '{id}' fue actualizado por otro usuario.")
    {
    }
}
