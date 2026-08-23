using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.TiposCambio;

using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class TipoCambioRepository : ITipoCambioRepository
{
    private readonly LicitacionesDbContext _context;

    public TipoCambioRepository(LicitacionesDbContext context) =>
        _context = context;

    public Task<TipoCambio?> ObtenerActivoAsync(
        CancellationToken cancellationToken = default) =>
        _context.TiposCambio
            .SingleOrDefaultAsync(
                tipo => tipo.Activo
                    && tipo.MonedaOrigen == TipoCambio.MonedaOrigenPredeterminada
                    && tipo.MonedaDestino == TipoCambio.MonedaDestinoPredeterminada,
                cancellationToken);

    public async Task<IReadOnlyList<TipoCambio>> ListarTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var tipos = await _context.TiposCambio
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return tipos;
    }

    public async Task ReemplazarActivoAsync(
        TipoCambio tipoCambio,
        CancellationToken cancellationToken = default)
    {
        var activos = await _context.TiposCambio
            .Where(tipo => tipo.Activo)
            .ToListAsync(cancellationToken);

        foreach (var activo in activos)
        {
            activo.Desactivar();
        }

        await _context.TiposCambio.AddAsync(tipoCambio, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
