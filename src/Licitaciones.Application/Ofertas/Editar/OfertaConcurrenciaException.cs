namespace Licitaciones.Application.Ofertas.Editar;

/// <summary>
/// Excepción que se lanza cuando una oferta fue modificada por otro usuario.
/// </summary>
public sealed class OfertaConcurrenciaException : Exception
{
    public OfertaConcurrenciaException(Guid id)
        : base($"La oferta '{id}' fue actualizada por otro usuario.")
    {
    }

    public OfertaConcurrenciaException()
        : base("La oferta fue actualizada por otro usuario. Intente nuevamente.")
    {
    }
}
