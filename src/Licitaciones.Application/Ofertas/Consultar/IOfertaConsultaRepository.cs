using Licitaciones.Application.TiposCambio;

namespace Licitaciones.Application.Ofertas.Consultar;

public interface IOfertaConsultaRepository
{
    Task<IReadOnlyList<OfertaConsultaRegistro>> ListarAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default);

    Task<OfertaConsultaRegistro?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TipoCambioDto?> ObtenerTipoCambioUsdCrcAsync(
        CancellationToken cancellationToken = default);
}

