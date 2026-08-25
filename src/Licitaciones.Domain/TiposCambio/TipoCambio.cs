using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.TiposCambio;

/// <summary>
/// Entidad que almacena un registro de tipo de cambio entre dos monedas para una fecha específica.
/// </summary>
public sealed class TipoCambio : IAuditableEntity
{
    /// <summary>
    /// Código de moneda origen predeterminado (USD).
    /// </summary>
    public const string MonedaOrigenPredeterminada = "USD";

    /// <summary>
    /// Código de moneda destino predeterminado (CRC - Colón costarricense).
    /// </summary>
    public const string MonedaDestinoPredeterminada = "CRC";

    private TipoCambio()
    {
    }

    /// <summary>
    /// Identificador del registro de tipo de cambio.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Código de la moneda origen (ej. USD).
    /// </summary>
    public string MonedaOrigen { get; private set; } = string.Empty;

    /// <summary>
    /// Código de la moneda destino (ej. CRC).
    /// </summary>
    public string MonedaDestino { get; private set; } = string.Empty;

    /// <summary>
    /// Valor del tipo de cambio (unidades de moneda destino por unidad de moneda origen).
    /// </summary>
    public decimal Valor { get; private set; }

    /// <summary>
    /// Fecha a la que corresponde el tipo de cambio.
    /// </summary>
    public DateOnly Fecha { get; private set; }

    /// <summary>
    /// Indica si el registro está activo.
    /// </summary>
    public bool Activo { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Crea un nuevo registro de tipo de cambio USD→CRC para la fecha indicada.
    /// </summary>
    /// <param name="valor">Valor del tipo de cambio (debe ser mayor que cero).</param>
    /// <param name="fecha">Fecha a la que corresponde el tipo de cambio.</param>
    /// <returns>Nueva instancia de <see cref="TipoCambio"/>.</returns>
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

    /// <summary>
    /// Desactiva el registro de tipo de cambio.
    /// </summary>
    public void Desactivar() => Activo = false;

    /// <summary>
    /// Activa el registro de tipo de cambio.
    /// </summary>
    public void Activar() => Activo = true;

    /// <summary>
    /// Actualiza el valor y la fecha del tipo de cambio.
    /// </summary>
    /// <param name="valor">Nuevo valor del tipo de cambio (debe ser mayor que cero).</param>
    /// <param name="fecha">Nueva fecha del tipo de cambio.</param>
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
