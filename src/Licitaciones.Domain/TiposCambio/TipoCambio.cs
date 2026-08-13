using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.TiposCambio;

public sealed class TipoCambio : IAuditableEntity
{
    private TipoCambio()
    {
    }

    public int Id { get; private set; }
    public string MonedaOrigen { get; private set; } = string.Empty;
    public string MonedaDestino { get; private set; } = string.Empty;
    public decimal Valor { get; private set; }
    public DateOnly Fecha { get; private set; }
    public bool Activo { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
}
