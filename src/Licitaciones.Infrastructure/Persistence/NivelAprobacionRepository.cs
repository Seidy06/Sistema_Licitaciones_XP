using Licitaciones.Application.Aprobaciones;
using Licitaciones.Domain.Aprobaciones;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

/// <summary>
/// Repositorio de niveles de aprobación con validación de traslape de rangos.
/// </summary>
public sealed class NivelAprobacionRepository : INivelAprobacionRepository
{
    private const string RestriccionSinTraslape = "EX_NivelesAprobacion_SinTraslape";
    private readonly LicitacionesDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de niveles de aprobación.
    /// </summary>
    public NivelAprobacionRepository(LicitacionesDbContext context) =>
        _context = context;

    /// <summary>
    /// Verifica si existe un nivel activo cuyo rango se traslape con el indicado.
    /// </summary>
    public Task<bool> ExisteTraslapeActivoAsync(
        decimal montoMinimo,
        decimal? montoMaximo,
        int? excludeId = null,
        CancellationToken cancellationToken = default) =>
        _context.NivelesAprobacion.AnyAsync(
            nivel => nivel.Activo
                && (excludeId == null || nivel.Id != excludeId.Value)
                && (nivel.MontoMaximo == null || nivel.MontoMaximo > montoMinimo)
                && (montoMaximo == null || nivel.MontoMinimo < montoMaximo),
            cancellationToken);

    /// <summary>
    /// Agrega un nuevo nivel de aprobación y persiste los cambios.
    /// </summary>
    public async Task AgregarAsync(
        NivelAprobacion nivel,
        CancellationToken cancellationToken = default)
    {
        _context.NivelesAprobacion.Add(nivel);
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (EsTraslape(exception))
        {
            throw new NivelAprobacionConflictoException();
        }
    }

    /// <summary>
    /// Lista todos los niveles de aprobación activos.
    /// </summary>
    public async Task<IReadOnlyList<NivelAprobacion>> ListarActivosAsync(
        CancellationToken cancellationToken = default)
    {
        var niveles = await _context.NivelesAprobacion
            .AsNoTracking()
            .Where(nivel => nivel.Activo)
            .ToListAsync(cancellationToken);

        return niveles;
    }

    /// <summary>
    /// Obtiene un nivel de aprobación por su identificador.
    /// </summary>
    public Task<NivelAprobacion?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _context.NivelesAprobacion.FirstOrDefaultAsync(
            nivel => nivel.Id == id, cancellationToken);

    /// <summary>
    /// Persiste todos los cambios pendientes en el contexto.
    /// </summary>
    public Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);

    private static bool EsTraslape(DbUpdateException exception) =>
        exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.ExclusionViolation,
            ConstraintName: RestriccionSinTraslape
        };
}
