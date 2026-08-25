using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Ofertas.Editar;
using Licitaciones.Application.Ofertas.Eliminar;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;
using Licitaciones.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class OfertaRepository :
    IOfertaRepository,
    IEditarOfertaRepository,
    IEliminarOfertaRepository,
    IOfertaConsultaRepository
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

    public Task<Licitacion?> ObtenerLicitacionPorOfertaIdAsync(
        Guid ofertaId,
        CancellationToken cancellationToken = default) =>
        _context.Ofertas
            .Where(oferta => oferta.Id == ofertaId)
            .Join(
                _context.Licitaciones,
                oferta => oferta.LicitacionId,
                licitacion => licitacion.Id,
                (_, licitacion) => licitacion)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<OfertaConsultaRegistro>> ListarAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default) =>
        await ProyectarConsulta(
                _context.Ofertas
                    .Where(oferta => oferta.LicitacionId == licitacionId)
                    .OrderBy(oferta => oferta.Monto)
                    .ThenBy(oferta => oferta.FechaRegistro))
            .ToListAsync(cancellationToken);

    public Task<OfertaConsultaRegistro?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        ProyectarConsulta(
                _context.Ofertas.Where(oferta => oferta.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AgregarAsync(
        Oferta oferta, CancellationToken cancellationToken = default) =>
        await _context.Ofertas.AddAsync(oferta, cancellationToken);

    public Task<Oferta?> ObtenerEntidadPorIdAsync(
        Guid id, CancellationToken cancellationToken = default) =>
        _context.Ofertas.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    Task<Licitacion?> IEditarOfertaRepository.ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken) =>
        _context.Licitaciones.FirstOrDefaultAsync(
            x => x.Id == id && x.DeletedAt == null, cancellationToken);

    Task<Licitacion?> IEliminarOfertaRepository.ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken) =>
        _context.Licitaciones.FirstOrDefaultAsync(
            x => x.Id == id && x.DeletedAt == null, cancellationToken);

    Task<Oferta?> IEditarOfertaRepository.ObtenerPorIdAsync(
        Guid id, CancellationToken cancellationToken) =>
        ObtenerEntidadPorIdAsync(id, cancellationToken);

    Task<Oferta?> IEliminarOfertaRepository.ObtenerPorIdAsync(
        Guid id, CancellationToken cancellationToken) =>
        ObtenerEntidadPorIdAsync(id, cancellationToken);

    private IQueryable<OfertaConsultaRegistro> ProyectarConsulta(
        IQueryable<Oferta> ofertas) =>
        ofertas.Join(
            _context.Proveedores,
            oferta => oferta.ProveedorId,
            proveedor => proveedor.Id,
            (oferta, proveedor) => new OfertaConsultaRegistro(
                oferta.Id,
                oferta.LicitacionId,
                proveedor.Nombre,
                oferta.Monto,
                oferta.FechaRegistro));

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
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: OfertaConfiguration.IndiceUnicoLicitacionProveedor
            })
        {
            throw new OfertaDuplicadaException();
        }
    }
}
