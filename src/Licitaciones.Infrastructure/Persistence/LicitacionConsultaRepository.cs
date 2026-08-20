using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class LicitacionConsultaRepository : ILicitacionConsultaRepository
{
    private readonly LicitacionesDbContext _context;
    private readonly IClock _clock;

    public LicitacionConsultaRepository(
        LicitacionesDbContext context,
        IClock clock)
    {
        _context = context;
        _clock = clock;
    }

    public async Task<IReadOnlyList<Licitacion>> ListarAsync(
        ConsultarLicitacionesRequest consulta,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Licitacion> query = _context.Licitaciones
            .Where(l => l.DeletedAt == null);

        if (consulta.EstadoFiltro.HasValue)
        {
            var filtro = consulta.EstadoFiltro.Value;
            var ahora = _clock.UtcNow();

            query = filtro switch
            {
                EstadoLicitacion.Publicada => query.Where(l =>
                    l.Estado == EstadoLicitacion.Publicada
                    && l.FechaCierre > ahora),
                EstadoLicitacion.Cerrada => query.Where(l =>
                    l.Estado == EstadoLicitacion.Cerrada
                    || (l.Estado == EstadoLicitacion.Publicada
                        && l.FechaCierre <= ahora)),
                _ => query.Where(l => l.Estado == filtro)
            };
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        await _context.Licitaciones
            .FirstOrDefaultAsync(
                l => l.Id == id && l.DeletedAt == null,
                cancellationToken);

    public async Task<IReadOnlyList<Oferta>> ObtenerOfertasAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default) =>
        await _context.Ofertas
            .Where(o => o.LicitacionId == licitacionId)
            .ToListAsync(cancellationToken);

    public async Task<LicitacionNivelAprobacionDto?> ObtenerNivelAprobacionAsync(
        decimal montoOferta,
        CancellationToken cancellationToken = default)
    {
        var nivel = await _context.NivelesAprobacion
            .FirstOrDefaultAsync(n =>
                montoOferta >= n.MontoMinimo
                && (n.MontoMaximo == null || montoOferta < n.MontoMaximo),
                cancellationToken);

        return nivel is null
            ? null
            : new LicitacionNivelAprobacionDto(nivel.Id, nivel.Nombre);
    }
}
