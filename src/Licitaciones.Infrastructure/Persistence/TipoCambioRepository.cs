using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.TiposCambio;

using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Persistence;

/// <summary>
/// Repositorio de tipos de cambio con soporte de reemplazo del activo.
/// </summary>
public sealed class TipoCambioRepository : ITipoCambioRepository
{
    private readonly LicitacionesDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de tipos de cambio.
    /// </summary>
    public TipoCambioRepository(LicitacionesDbContext context) =>
        _context = context;

    /// <summary>
    /// Obtiene el tipo de cambio activo predeterminado (USD a VES).
    /// </summary>
    public Task<TipoCambio?> ObtenerActivoAsync(
        CancellationToken cancellationToken = default) =>
        _context.TiposCambio
            .SingleOrDefaultAsync(
                tipo => tipo.Activo
                    && tipo.MonedaOrigen == TipoCambio.MonedaOrigenPredeterminada
                    && tipo.MonedaDestino == TipoCambio.MonedaDestinoPredeterminada,
                cancellationToken);

    /// <summary>
    /// Obtiene un tipo de cambio por su identificador.
    /// </summary>
    public Task<TipoCambio?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _context.TiposCambio.FirstOrDefaultAsync(
            tipo => tipo.Id == id, cancellationToken);

    /// <summary>
    /// Lista todos los tipos de cambio registrados.
    /// </summary>
    public async Task<IReadOnlyList<TipoCambio>> ListarTodosAsync(
        CancellationToken cancellationToken = default)
    {
        var tipos = await _context.TiposCambio
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return tipos;
    }

    /// <summary>
    /// Desactiva todos los tipos de cambio activos y agrega uno nuevo.
    /// </summary>
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

    /// <summary>
    /// Persiste todos los cambios pendientes en el contexto.
    /// </summary>
    public Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
