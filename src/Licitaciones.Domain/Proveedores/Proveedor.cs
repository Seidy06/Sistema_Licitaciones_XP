using System.Text;
using System.Text.RegularExpressions;

namespace Licitaciones.Domain.Proveedores;

public sealed class Proveedor
{
    private Proveedor()
    {
    }

    public Guid Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;

    public string NombreNormalizado { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public uint Version { get; private set; }

    public static Proveedor Crear(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new ArgumentException(
                "El nombre del proveedor es obligatorio.",
                nameof(nombre));
        }

        if (!ProveedorNombreValidator.EsValido(nombre))
        {
            throw new ArgumentException(
                "El nombre del proveedor contiene caracteres no permitidos.",
                nameof(nombre));
        }

        var nombreLegible = Regex.Replace(
            nombre.Normalize(NormalizationForm.FormC).Trim(),
            @"\s+",
            " ");
        var ahora = DateTimeOffset.UtcNow;

        return new Proveedor
        {
            Id = Guid.NewGuid(),
            Nombre = nombreLegible,
            NombreNormalizado = ProveedorNombreNormalizer.Normalizar(nombreLegible),
            CreatedAt = ahora,
            UpdatedAt = ahora,
            Version = 0
        };
    }
}
