namespace Licitaciones.Application.Licitaciones.Editar;

/// <summary>
/// Excepción que se lanza cuando una licitación fue modificada por otro usuario.
/// </summary>
public sealed class LicitacionConcurrenciaException : Exception
{
    public LicitacionConcurrenciaException(Guid id)
        : base($"La licitación '{id}' fue actualizada por otro usuario.")
    {
    }

    public LicitacionConcurrenciaException()
        : base("La licitación fue actualizada por otro usuario. Intente nuevamente.")
    {
    }
}
