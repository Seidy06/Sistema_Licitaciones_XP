using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Proveedores;

public sealed class Proveedor : IAuditableEntity
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
        var (nombreLegible, nombreNormalizado) =
            ProveedorNombreNormalizer.NormalizarAmbos(nombre);

        if (!ProveedorNombreValidator.EsValido(nombreLegible))
        {
            throw new ArgumentException(
                "El nombre del proveedor contiene caracteres no permitidos.",
                nameof(nombre));
        }

        return new Proveedor
        {
            Id = Guid.NewGuid(),
            Nombre = nombreLegible,
            NombreNormalizado = nombreNormalizado,
            Version = 0
        };
    }

    public void Editar(string nombre)
    {
        var (nombreLegible, nombreNormalizado) =
            ProveedorNombreNormalizer.NormalizarAmbos(nombre);

        if (!ProveedorNombreValidator.EsValido(nombreLegible))
        {
            throw new ArgumentException(
                "El nombre del proveedor contiene caracteres no permitidos.",
                nameof(nombre));
        }

        Nombre = nombreLegible;
        NombreNormalizado = nombreNormalizado;
    }
}
