using System.Globalization;

namespace Licitaciones.Web;

public static class FormatoMonetario
{
    private static readonly CultureInfo CulturaCostaRica = CrearCultura();

    public static string Dinero(this decimal valor) =>
        valor.ToString("C", CulturaCostaRica);

    public static string Dinero(this decimal valor, string moneda) =>
        string.Equals(moneda, "USD", StringComparison.OrdinalIgnoreCase)
            ? $"{valor.ToString("N2", CulturaCostaRica)} US$"
            : valor.Dinero();

    public static string Dinero(this decimal? valor, string textoAlternativo) =>
        valor.HasValue ? valor.Value.Dinero() : textoAlternativo;

    private static CultureInfo CrearCultura()
    {
        var cultura = (CultureInfo)new CultureInfo("es-CR").Clone();
        cultura.NumberFormat.CurrencyGroupSeparator = ".";
        return CultureInfo.ReadOnly(cultura);
    }
}
