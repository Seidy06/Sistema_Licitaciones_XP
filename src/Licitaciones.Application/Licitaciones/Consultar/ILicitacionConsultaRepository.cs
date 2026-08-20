using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Licitaciones.Consultar;

public interface ILicitacionConsultaRepository
{
    Task<IReadOnlyList<Licitacion>> ListarAsync(
        ConsultarLicitacionesRequest consulta,
        CancellationToken cancellationToken = default);

    Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Oferta>> ObtenerOfertasAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default);

    Task<LicitacionNivelAprobacionDto?> ObtenerNivelAprobacionAsync(
        decimal montoOferta,
        CancellationToken cancellationToken = default);
}
