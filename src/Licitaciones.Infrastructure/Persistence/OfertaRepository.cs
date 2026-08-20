using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class OfertaRepository : IOfertaRepository
{
    private readonly LicitacionesDbContext _context;

    public OfertaRepository(LicitacionesDbContext context) => _context = context;

    public Task<Licitacion?> ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken = default) =>
        _context.Licitaciones.FirstOrDefaultAsync(
            x => x.Id == id && x.DeletedAt == null, cancellationToken);

    public Task<Proveedor?> ObtenerProveedorPorIdAsync(
        Guid id, CancellationToken cancellationToken = default) =>
        _context.Proveedores.FirstOrDefaultAsync(
            x => x.Id == id && x.DeletedAt == null, cancellationToken);

    public Task<bool> ExisteOfertaAsync(
        Guid licitacionId, Guid proveedorId,
        CancellationToken cancellationToken = default) =>
        _context.Ofertas.AnyAsync(
            x => x.LicitacionId == licitacionId && x.ProveedorId == proveedorId,
            cancellationToken);

    public async Task AgregarAsync(
        Oferta oferta, CancellationToken cancellationToken = default) =>
        await _context.Ofertas.AddAsync(oferta, cancellationToken);

    public async Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation
            })
        {
            throw new DomainException(CrearOfertaService.ErrorDuplicada);
        }
    }
}
