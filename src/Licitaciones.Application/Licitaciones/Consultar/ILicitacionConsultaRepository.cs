using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Consultar;

public interface ILicitacionConsultaRepository
{
    Task<IReadOnlyList<Licitacion>> ListarAsync(
        ConsultarLicitacionesRequest consulta,
        CancellationToken cancellationToken = default);

    Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<decimal?> ObtenerMontoMinimoOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default);

    Task<LicitacionNivelAprobacionDto?> ObtenerNivelAprobacionAsync(
        decimal montoOferta,
        CancellationToken cancellationToken = default);
}
