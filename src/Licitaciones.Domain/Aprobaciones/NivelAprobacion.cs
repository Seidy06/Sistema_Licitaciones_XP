using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Aprobaciones;

public sealed class NivelAprobacion : IAuditableEntity
{
    private NivelAprobacion()
    {
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public decimal MontoMinimo { get; private set; }
    public decimal? MontoMaximo { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
