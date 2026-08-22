namespace Licitaciones.Application.Aprobaciones;

public sealed class NivelAprobacionConflictoException : Exception
{
    public NivelAprobacionConflictoException()
        : base("El rango se traslapa con otro nivel de aprobación activo.")
    {
    }
}
