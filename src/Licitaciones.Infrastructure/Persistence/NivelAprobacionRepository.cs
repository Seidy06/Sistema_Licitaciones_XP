using Licitaciones.Application.Aprobaciones;
using Licitaciones.Domain.Aprobaciones;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class NivelAprobacionRepository : INivelAprobacionRepository
{
    private const string RestriccionSinTraslape = "EX_NivelesAprobacion_SinTraslape";
    private readonly LicitacionesDbContext _context;

    public NivelAprobacionRepository(LicitacionesDbContext context) =>
        _context = context;

    public Task<bool> ExisteTraslapeActivoAsync(
        decimal montoMinimo,
        decimal? montoMaximo,
        CancellationToken cancellationToken = default) =>
        _context.NivelesAprobacion.AnyAsync(
            nivel => nivel.Activo
                && (nivel.MontoMaximo == null || nivel.MontoMaximo > montoMinimo)
                && (montoMaximo == null || nivel.MontoMinimo < montoMaximo),
            cancellationToken);

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

    public async Task<IReadOnlyList<NivelAprobacion>> ListarActivosAsync(
        CancellationToken cancellationToken = default)
    {
        var niveles = await _context.NivelesAprobacion
            .AsNoTracking()
            .Where(nivel => nivel.Activo)
            .ToListAsync(cancellationToken);

        return niveles;
    }

    public Task<NivelAprobacion?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default) =>
        _context.NivelesAprobacion.FirstOrDefaultAsync(
            nivel => nivel.Id == id, cancellationToken);

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
