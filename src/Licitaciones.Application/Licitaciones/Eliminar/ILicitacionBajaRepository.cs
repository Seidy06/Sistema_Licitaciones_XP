using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Eliminar;

public interface ILicitacionBajaRepository
{
    Task<Licitacion?> ObtenerActivaParaDarDeBajaAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task ActualizarBajaAsync(
        Licitacion licitacion,
        CancellationToken cancellationToken = default);
}
