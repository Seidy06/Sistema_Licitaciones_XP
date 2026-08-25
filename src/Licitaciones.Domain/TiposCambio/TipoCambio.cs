using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.TiposCambio;

public sealed class TipoCambio : IAuditableEntity
{
    public const string MonedaOrigenPredeterminada = "USD";
    public const string MonedaDestinoPredeterminada = "CRC";

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

    public static TipoCambio Crear(decimal valor, DateOnly fecha)
    {
        if (valor <= 0)
        {
            throw new DomainException("El valor del tipo de cambio debe ser mayor que cero.");
        }

        return new TipoCambio
        {
            MonedaOrigen = MonedaOrigenPredeterminada,
            MonedaDestino = MonedaDestinoPredeterminada,
            Valor = valor,
            Fecha = fecha,
            Activo = true
        };
    }

    public void Desactivar() => Activo = false;

    public void Activar() => Activo = true;

    public void Actualizar(decimal valor, DateOnly fecha)
    {
        if (valor <= 0)
        {
            throw new DomainException("El valor del tipo de cambio debe ser mayor que cero.");
        }

        Valor = valor;
        Fecha = fecha;
    }
}
