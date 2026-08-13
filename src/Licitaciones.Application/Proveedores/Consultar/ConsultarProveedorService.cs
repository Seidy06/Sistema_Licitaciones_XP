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

    private static ProveedorDto Mapear(Proveedor proveedor) => new(
        proveedor.Id,
        proveedor.Nombre,
        proveedor.NombreNormalizado,
        proveedor.CreatedAt,
        proveedor.UpdatedAt,
        proveedor.Version);
}
