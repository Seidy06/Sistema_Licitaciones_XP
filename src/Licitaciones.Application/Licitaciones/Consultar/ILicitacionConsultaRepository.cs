using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Licitaciones.Consultar;

/// <summary>
/// Puerto de salida para la consulta de licitaciones en el contexto de solo lectura.
/// </summary>
public interface ILicitacionConsultaRepository
{
    /// <summary>
    /// Lista licitaciones según los filtros de búsqueda proporcionados.
    /// </summary>
    Task<IReadOnlyList<Licitacion>> ListarAsync(
        ConsultarLicitacionesRequest consulta,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una licitación por su identificador.
    /// </summary>
    Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene las ofertas asociadas a una licitación.
    /// </summary>
    Task<IReadOnlyList<Oferta>> ObtenerOfertasAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el nivel de aprobación aplicable según el monto de la oferta.
    /// </summary>
    Task<LicitacionNivelAprobacionDto?> ObtenerNivelAprobacionAsync(
        decimal montoOferta,
        CancellationToken cancellationToken = default);
}
