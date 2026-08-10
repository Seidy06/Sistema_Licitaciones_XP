using System.Text;
using System.Text.RegularExpressions;

namespace Licitaciones.Domain.Proveedores;

public static class ProveedorNombreNormalizer
{
    public static string Normalizar(string nombre)
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

        return normalizado.ToUpperInvariant();
    }
}
