using Licitaciones.Application.Proveedores;
using Licitaciones.Domain.Proveedores;
using Microsoft.EntityFrameworkCore;

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
        await _context.SaveChangesAsync(cancellationToken);
    }
}
