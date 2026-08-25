using System.Globalization;

namespace Licitaciones.Web;

/// <summary>
/// Clase auxiliar con métodos de extensión para formatear valores monetarios en colones costarricenses.
/// </summary>
public static class FormatoMonetario
{
    private static readonly CultureInfo CulturaCostaRica = CrearCultura();

    /// <summary>
    /// Formatea un valor decimal como moneda en colones costarricenses (₡).
    /// </summary>
    public static string Dinero(this decimal valor) =>
        valor.ToString("C", CulturaCostaRica);

    /// <summary>
    /// Formatea un valor decimal en la moneda especificada (CRC o USD).
    /// </summary>
    public static string Dinero(this decimal valor, string moneda) =>
        string.Equals(moneda, "USD", StringComparison.OrdinalIgnoreCase)
            ? $"{valor.ToString("N2", CulturaCostaRica)} US$"
            : valor.Dinero();

    /// <summary>
    /// Formatea un valor nullable como moneda, o retorna un texto alternativo si es nulo.
    /// </summary>
    public static string Dinero(this decimal? valor, string textoAlternativo) =>
        valor.HasValue ? valor.Value.Dinero() : textoAlternativo;

    private static CultureInfo CrearCultura()
    {
        var cultura = (CultureInfo)new CultureInfo("es-CR").Clone();
        cultura.NumberFormat.CurrencyGroupSeparator = ".";
        return CultureInfo.ReadOnly(cultura);
    }
}
