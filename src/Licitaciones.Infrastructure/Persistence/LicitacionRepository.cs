using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class LicitacionRepository : ILicitacionRepository
{
    private readonly LicitacionesDbContext _context;

    public LicitacionRepository(LicitacionesDbContext context) => _context = context;

    public Task<bool> ExisteCodigoNormalizadoAsync(
        string codigoNormalizado,
        CancellationToken cancellationToken = default) =>
        _context.Licitaciones.AnyAsync(
            licitacion => licitacion.CodigoNormalizado == codigoNormalizado
                && licitacion.DeletedAt == null,
            cancellationToken);

    public async Task AgregarAsync(
        Licitacion licitacion,
        CancellationToken cancellationToken = default)
    {
        await _context.Licitaciones.AddAsync(licitacion, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: LicitacionConfiguration.IndiceUnicoCodigoNormalizado
            })
        {
            throw new LicitacionDuplicadoException(licitacion.Codigo);
        }
    }

    public Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        _context.Licitaciones
            .FirstOrDefaultAsync(
                l => l.Id == id && l.DeletedAt == null,
                cancellationToken);

    public async Task<decimal?> ObtenerMontoMinimoOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default) =>
        await _context.Ofertas
            .Where(o => o.LicitacionId == licitacionId)
            .MinAsync(o => (decimal?)o.Monto, cancellationToken);

    public Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
