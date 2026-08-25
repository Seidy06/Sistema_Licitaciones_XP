using System.Text.RegularExpressions;

namespace Licitaciones.Domain.Proveedores;

/// <summary>
/// Valida que el nombre de un proveedor cumpla con las reglas de caracteres permitidos.
/// </summary>
public static class ProveedorNombreValidator
{
    private static readonly Regex NombreRegex =
        new(
            @"^[\p{L}\p{N} .,\(\)]+$",
            RegexOptions.Compiled);

    /// <summary>
    /// Determina si el nombre contiene únicamente caracteres permitidos (letras, números, espacios, puntos, comas y paréntesis).
    /// </summary>
    /// <param name="nombre">Nombre a validar.</param>
    /// <returns>true si el nombre es válido; de lo contrario, false.</returns>
    public static bool EsValido(string nombre)
    {
        return !string.IsNullOrWhiteSpace(nombre)
            && NombreRegex.IsMatch(nombre);
    }
}
