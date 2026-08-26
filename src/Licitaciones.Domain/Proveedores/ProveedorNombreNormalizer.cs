using System.Text;
using System.Text.RegularExpressions;

namespace Licitaciones.Domain.Proveedores;

/// <summary>
/// Normaliza los nombres de proveedores eliminando espacios duplicados y aplicando formato consistente.
/// </summary>
public static class ProveedorNombreNormalizer
{
    /// <summary>
    /// Normaliza el nombre y retorna la versión legible (con formato).
    /// </summary>
    /// <param name="nombre">Nombre a normalizar.</param>
    /// <returns>Nombre normalizado en formato legible.</returns>
    public static string NormalizarLegible(string nombre)
    {
        return NormalizarAmbos(nombre).NombreLegible;
    }

    /// <summary>
    /// Normaliza el nombre y retorna la versión en mayúsculas para indexación.
    /// </summary>
    /// <param name="nombre">Nombre a normalizar.</param>
    /// <returns>Nombre normalizado en mayúsculas.</returns>
    public static string Normalizar(string nombre)
    {
        return NormalizarAmbos(nombre).NombreNormalizado;
    }

    internal static (string NombreLegible, string NombreNormalizado) NormalizarAmbos(
        string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre del proveedor es obligatorio.",
                nameof(nombre));
        }

        var normalizado = nombre
            .Normalize(NormalizationForm.FormC)
            .Trim();

        normalizado = Regex.Replace(normalizado, @"\s+", " ");

        return (normalizado, normalizado.ToUpperInvariant());
    }
}
