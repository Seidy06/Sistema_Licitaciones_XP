using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Application.Licitaciones.Eliminar;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

/// <summary>
/// Repositorio de licitaciones con operaciones CRUD y baja lógica.
/// </summary>
public sealed class LicitacionRepository : ILicitacionRepository, ILicitacionBajaRepository
{
    private readonly LicitacionesDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de licitaciones.
    /// </summary>
    public LicitacionRepository(LicitacionesDbContext context) => _context = context;

    /// <summary>
    /// Verifica si ya existe una licitación activa con el código normalizado dado.
    /// </summary>
    public Task<bool> ExisteCodigoNormalizadoAsync(
        string codigoNormalizado,
        CancellationToken cancellationToken = default) =>
        _context.Licitaciones.AnyAsync(
            licitacion => licitacion.CodigoNormalizado == codigoNormalizado
                && licitacion.DeletedAt == null,
            cancellationToken);

    /// <summary>
    /// Agrega una nueva licitación y persiste los cambios.
    /// </summary>
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

    /// <summary>
    /// Obtiene una licitación activa por su identificador.
    /// </summary>
    public Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        _context.Licitaciones
            .FirstOrDefaultAsync(
                l => l.Id == id && l.DeletedAt == null,
                cancellationToken);

    /// <summary>
    /// Obtiene el monto mínimo entre todas las ofertas de una licitación.
    /// </summary>
    public async Task<decimal?> ObtenerMontoMinimoOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default) =>
        await _context.Ofertas
            .Where(o => o.LicitacionId == licitacionId)
            .MinAsync(o => (decimal?)o.Monto, cancellationToken);

    /// <summary>
    /// Persiste todos los cambios pendientes en el contexto.
    /// </summary>
    public async Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new LicitacionConcurrenciaException();
        }
    }

    /// <summary>
    /// Obtiene una licitación activa para dar de baja lógica.
    /// </summary>
    public Task<Licitacion?> ObtenerActivaParaDarDeBajaAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Licitaciones.SingleOrDefaultAsync(
            licitacion => licitacion.Id == id && licitacion.DeletedAt == null,
            cancellationToken);
    }

    /// <summary>
    /// Persiste la baja lógica de una licitación.
    /// </summary>
    public async Task ActualizarBajaAsync(
        Licitacion licitacion,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new LicitacionConcurrenciaException();
        }
    }
}
