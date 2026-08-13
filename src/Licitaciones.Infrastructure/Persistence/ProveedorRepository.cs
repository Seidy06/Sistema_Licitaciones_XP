using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Proveedores;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class ProveedorRepository : IProveedorRepository
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
            throw new ProveedorDuplicadoException(proveedor.Nombre);
        }
    }

    private static bool EsConflictoDeNombreNormalizado(
        DbUpdateException exception)
    {
        return exception.InnerException is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation,
            ConstraintName: "UX_Proveedores_NombreNormalizado"
        };
    }
}
