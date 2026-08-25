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

/// <summary>
/// Repositorio de ofertas con operaciones CRUD, consulta y eliminación.
/// </summary>
public sealed class OfertaRepository :
    IOfertaRepository,
    IEditarOfertaRepository,
    IEliminarOfertaRepository,
    IOfertaConsultaRepository
{
    private readonly LicitacionesDbContext _context;

    /// <summary>
    /// Inicializa una nueva instancia del repositorio de ofertas.
    /// </summary>
    public OfertaRepository(LicitacionesDbContext context) => _context = context;

    /// <summary>
    /// Obtiene una licitación activa por su identificador.
    /// </summary>
    public Task<Licitacion?> ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken = default) =>
        _context.Licitaciones.FirstOrDefaultAsync(
            x => x.Id == id && x.DeletedAt == null, cancellationToken);

    /// <summary>
    /// Obtiene un proveedor activo por su identificador.
    /// </summary>
    public Task<Proveedor?> ObtenerProveedorPorIdAsync(
        Guid id, CancellationToken cancellationToken = default) =>
        _context.Proveedores.FirstOrDefaultAsync(
            x => x.Id == id && x.DeletedAt == null, cancellationToken);

    /// <summary>
    /// Verifica si ya existe una oferta para la licitación y proveedor indicados.
    /// </summary>
    public Task<bool> ExisteOfertaAsync(
        Guid licitacionId, Guid proveedorId,
        CancellationToken cancellationToken = default) =>
        _context.Ofertas.AnyAsync(
            x => x.LicitacionId == licitacionId && x.ProveedorId == proveedorId,
            cancellationToken);

    /// <summary>
    /// Obtiene la licitación asociada a una oferta por su identificador.
    /// </summary>
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

    /// <summary>
    /// Lista todas las ofertas de una licitación ordenadas por monto y fecha.
    /// </summary>
    public async Task<IReadOnlyList<OfertaConsultaRegistro>> ListarAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default) =>
        await ProyectarConsulta(
                _context.Ofertas
                    .Where(oferta => oferta.LicitacionId == licitacionId)
                    .OrderBy(oferta => oferta.Monto)
                    .ThenBy(oferta => oferta.FechaRegistro))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Lista todas las ofertas de un proveedor ordenadas por fecha de registro.
    /// </summary>
    public async Task<IReadOnlyList<OfertaConsultaRegistro>> ListarPorProveedorIdAsync(
        Guid proveedorId,
        CancellationToken cancellationToken = default) =>
        await ProyectarConsulta(
                _context.Ofertas
                    .Where(oferta => oferta.ProveedorId == proveedorId)
                    .OrderBy(oferta => oferta.FechaRegistro))
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Obtiene un registro de consulta de oferta por su identificador.
    /// </summary>
    public Task<OfertaConsultaRegistro?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        ProyectarConsulta(
                _context.Ofertas.Where(oferta => oferta.Id == id))
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>
    /// Agrega una nueva oferta al contexto de cambios.
    /// </summary>
    public async Task AgregarAsync(
        Oferta oferta, CancellationToken cancellationToken = default) =>
        await _context.Ofertas.AddAsync(oferta, cancellationToken);

    /// <summary>
    /// Obtiene la entidad de oferta por su identificador.
    /// </summary>
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
