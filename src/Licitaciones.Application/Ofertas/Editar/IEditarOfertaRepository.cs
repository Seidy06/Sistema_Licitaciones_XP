using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas.Editar;

public interface IEditarOfertaRepository
{
    Task<Oferta?> ObtenerPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Licitacion?> ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
