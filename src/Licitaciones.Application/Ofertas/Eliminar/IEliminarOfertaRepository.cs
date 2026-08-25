using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas.Eliminar;

public interface IEliminarOfertaRepository
{
    Task<Oferta?> ObtenerPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Licitacion?> ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
