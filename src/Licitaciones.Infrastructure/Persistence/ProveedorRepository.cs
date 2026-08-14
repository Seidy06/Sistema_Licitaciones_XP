using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Domain.Proveedores;
using Licitaciones.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class ProveedorRepository : IProveedorRepository, IProveedorConsultaRepository
{
    private readonly LicitacionesDbContext _context;

    public ProveedorRepository(LicitacionesDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExisteNombreNormalizadoAsync(
        string nombreNormalizado,
        CancellationToken cancellationToken = default)
    {
        return _context.Proveedores.AnyAsync(
            proveedor => proveedor.NombreNormalizado == nombreNormalizado,
            cancellationToken);
    }

    public async Task AgregarAsync(
        Proveedor proveedor,
        CancellationToken cancellationToken = default)
    {
        await _context.Proveedores.AddAsync(proveedor, cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (EsConflictoDeNombreNormalizado(exception))
        {
            throw new Licitaciones.Application.Proveedores.Crear.ProveedorDuplicadoException(
                proveedor.Nombre);
        }
    }

    public Task<Proveedor?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Proveedores
            .AsNoTracking()
            .SingleOrDefaultAsync(proveedor => proveedor.Id == id, cancellationToken);
    }

    public async Task<PaginaProveedores> ListarAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Proveedor> query = _context.Proveedores.AsNoTracking();

        if (consulta.Nombre is not null)
        {
            var nombreNormalizado = ProveedorNombreNormalizer.Normalizar(consulta.Nombre);
            query = query.Where(proveedor =>
                proveedor.NombreNormalizado.Contains(nombreNormalizado));
        }

        var total = await query.CountAsync(cancellationToken);
        var ordenada = Ordenar(query, consulta.OrdenarPor, consulta.Descendente);
        var items = await ordenada
            .Skip((consulta.Pagina - 1) * consulta.TamanoPagina)
            .Take(consulta.TamanoPagina)
            .ToListAsync(cancellationToken);

        return new PaginaProveedores(items, total);
    }

    public Task<Proveedor?> ObtenerParaEditarAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return _context.Proveedores.SingleOrDefaultAsync(
            proveedor => proveedor.Id == id,
            cancellationToken);
    }

    public Task<bool> ExisteNombreNormalizadoAsync(
        string nombreNormalizado,
        Guid excluirProveedorId,
        CancellationToken cancellationToken = default)
    {
        return _context.Proveedores.AnyAsync(
            proveedor => proveedor.Id != excluirProveedorId
                && proveedor.NombreNormalizado == nombreNormalizado,
            cancellationToken);
    }

    public async Task ActualizarAsync(
        Proveedor proveedor,
        uint versionEsperada,
        CancellationToken cancellationToken = default)
    {
        _context.Entry(proveedor).Property(item => item.Version).OriginalValue = versionEsperada;

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ProveedorConcurrenciaException(proveedor.Id);
        }
        catch (DbUpdateException exception)
            when (EsConflictoDeNombreNormalizado(exception))
        {
            throw new Licitaciones.Application.Proveedores.Editar.ProveedorDuplicadoException(
                proveedor.Nombre);
        }
    }

    private static IOrderedQueryable<Proveedor> Ordenar(
        IQueryable<Proveedor> query,
        ProveedorOrden ordenarPor,
        bool descendente)
    {
        return (ordenarPor, descendente) switch
        {
            (ProveedorOrden.FechaCreacion, true) => query
                .OrderByDescending(proveedor => proveedor.CreatedAt)
                .ThenBy(proveedor => proveedor.Id),
            (ProveedorOrden.FechaCreacion, false) => query
                .OrderBy(proveedor => proveedor.CreatedAt)
                .ThenBy(proveedor => proveedor.Id),
            (ProveedorOrden.Nombre, true) => query
                .OrderByDescending(proveedor => proveedor.NombreNormalizado)
                .ThenBy(proveedor => proveedor.Id),
            _ => query
                .OrderBy(proveedor => proveedor.NombreNormalizado)
                .ThenBy(proveedor => proveedor.Id)
        };
    }

    private static bool EsConflictoDeNombreNormalizado(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: ProveedorConfiguration.IndiceUnicoNombreNormalizado
        };
    }
}
