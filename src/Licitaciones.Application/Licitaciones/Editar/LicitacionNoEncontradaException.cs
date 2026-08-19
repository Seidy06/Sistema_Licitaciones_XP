namespace Licitaciones.Application.Licitaciones.Editar;

public sealed class LicitacionNoEncontradaException : Exception
{
    public LicitacionNoEncontradaException(Guid id)
        : base($"No se encontró la licitación '{id}'.")
    {
    }
}
