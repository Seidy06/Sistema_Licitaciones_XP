using System.Text.RegularExpressions;

namespace Licitaciones.Domain.Proveedores;

public static class ProveedorNombreValidator
{
    private static readonly Regex NombreRegex =
        new(
            @"^[\p{L}\p{N} .,\(\)]+$",
            RegexOptions.Compiled);

    public static bool EsValido(string nombre)
    {
        return !string.IsNullOrWhiteSpace(nombre)
            && NombreRegex.IsMatch(nombre);
    }
}
