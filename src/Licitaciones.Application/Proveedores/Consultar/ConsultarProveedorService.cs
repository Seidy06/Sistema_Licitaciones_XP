using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Consultar;

public sealed class ConsultarProveedorService
{
    private readonly IProveedorConsultaRepository _repository;

    public ConsultarProveedorService(IProveedorConsultaRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProveedorDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        return proveedor is null ? null : Mapear(proveedor);
    }

    public async Task<ProveedorHistoricoDto?> ObtenerHistoricoPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _repository.ObtenerHistoricoPorIdAsync(id, cancellationToken);
        return proveedor is null ? null : MapearHistorico(proveedor);
    }

    public async Task<PaginaResultado<ProveedorDto>> ListarAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _repository.ListarAsync(consulta, cancellationToken);

        return new PaginaResultado<ProveedorDto>(
            resultado.Items.Select(Mapear).ToArray(),
            resultado.Total,
            consulta.Pagina,
            consulta.TamanoPagina);
    }

    public async Task<PaginaResultado<ProveedorHistoricoDto>> ListarHistoricoAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _repository.ListarHistoricoAsync(consulta, cancellationToken);

        return new PaginaResultado<ProveedorHistoricoDto>(
            resultado.Items.Select(MapearHistorico).ToArray(),
            resultado.Total,
            consulta.Pagina,
            consulta.TamanoPagina);
    }

    private static ProveedorDto Mapear(Proveedor proveedor) => new(
        proveedor.Id,
        proveedor.Nombre,
        proveedor.NombreNormalizado,
        proveedor.CreatedAt,
        proveedor.UpdatedAt,
        proveedor.Version);

    private static ProveedorHistoricoDto MapearHistorico(Proveedor proveedor) => new(
        proveedor.Id,
        proveedor.Nombre,
        proveedor.NombreNormalizado,
        proveedor.CreatedAt,
        proveedor.UpdatedAt,
        proveedor.DeletedAt!.Value,
        proveedor.Version);
}
