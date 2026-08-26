using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Proveedores;

/// <summary>
/// Entidad que representa un proveedor registrado en el sistema.
/// </summary>
public sealed class Proveedor : IAuditableEntity
{
    private Proveedor()
    {
    }

    /// <summary>
    /// Identificador único del proveedor.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Nombre comercial del proveedor.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Nombre normalizado en mayúsculas para búsquedas.
    /// </summary>
    public string NombreNormalizado { get; private set; } = string.Empty;

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Fecha de baja lógica del proveedor, o null si está activo.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>
    /// Indica si el proveedor ha sido eliminado lógicamente.
    /// </summary>
    public bool EstaEliminado => DeletedAt.HasValue;

    /// <summary>
    /// Número de versión para control de concurrencia optimista.
    /// </summary>
    public uint Version { get; private set; }

    /// <summary>
    /// Crea un nuevo proveedor validando y normalizando el nombre.
    /// </summary>
    /// <param name="nombre">Nombre del proveedor.</param>
    /// <returns>Nueva instancia de <see cref="Proveedor"/>.</returns>
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

    /// <summary>
    /// Actualiza el nombre del proveedor después de validarlo y normalizarlo.
    /// </summary>
    /// <param name="nombre">Nuevo nombre del proveedor.</param>
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

    /// <summary>
    /// Marca el proveedor como eliminado lógicamente en el momento indicado.
    /// </summary>
    /// <param name="instante">Fecha y hora de la baja.</param>
    public void DarDeBaja(DateTimeOffset instante)
    {
        DeletedAt ??= instante;
    }
}
