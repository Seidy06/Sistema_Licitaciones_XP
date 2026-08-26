namespace Licitaciones.Application.Licitaciones.Eliminar;

public sealed class LicitacionNoEncontradaParaBajaException : Exception
{
    public LicitacionNoEncontradaParaBajaException(Guid id)
        : base($"No se encontró la licitación '{id}' para dar de baja.")
    {
    }
}
