namespace Licitaciones.Application.Ofertas.Consultar;

/// <summary>
/// Puerto de salida para la consulta de ofertas en el contexto de solo lectura.
/// </summary>
public interface IOfertaConsultaRepository
{
    /// <summary>
    /// Lista las ofertas registradas para una licitación específica.
    /// </summary>
    Task<IReadOnlyList<OfertaConsultaRegistro>> ListarAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista las ofertas registradas por un proveedor específico.
    /// </summary>
    Task<IReadOnlyList<OfertaConsultaRegistro>> ListarPorProveedorIdAsync(
        Guid proveedorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una oferta por su identificador.
    /// </summary>
    Task<OfertaConsultaRegistro?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}

