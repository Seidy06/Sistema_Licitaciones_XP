using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Ofertas.Proteger;

public interface IProteccionOfertaRepository
{
    Task<Licitacion?> ObtenerLicitacionPorOfertaIdAsync(
        Guid ofertaId,
        CancellationToken cancellationToken = default);
}
