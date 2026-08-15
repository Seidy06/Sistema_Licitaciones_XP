using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Domain.Proveedores;

namespace Licitaciones.UnitTests.Proveedores;

public sealed class ConsultarProveedorServiceTests
{
    [Fact]
    [Trait("HU", "HU-09")]
    public async Task ObtenerPorIdAsync_Existente_DebeRetornarDtoYNoEntidad()
    {
        var proveedor = Proveedor.Crear("Proveedor Central");
        var repository = new RepositorioConsultaEnMemoria { PorId = proveedor };
        var service = new ConsultarProveedorService(repository);

        var resultado = await service.ObtenerPorIdAsync(proveedor.Id);

        Assert.NotNull(resultado);
        Assert.IsType<ProveedorDto>(resultado);
        Assert.Equal(proveedor.Id, resultado.Id);
        Assert.Equal("Proveedor Central", resultado.Nombre);
    }

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task ObtenerPorIdAsync_Inexistente_DebeRetornarNull()
    {
        var service = new ConsultarProveedorService(new RepositorioConsultaEnMemoria());

        var resultado = await service.ObtenerPorIdAsync(Guid.NewGuid());

        Assert.Null(resultado);
    }

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task ListarAsync_DebeAplicarPaginacionFiltroYOrdenSolicitados()
    {
        var repository = new RepositorioConsultaEnMemoria
        {
            Pagina = new PaginaProveedores(
                [Proveedor.Crear("Beta")],
                total: 7)
        };
        var service = new ConsultarProveedorService(repository);
        var consulta = new ConsultarProveedoresRequest(
            pagina: 2,
            tamanoPagina: 3,
            nombre: "bEtA",
            ordenarPor: ProveedorOrden.FechaCreacion,
            descendente: true);

        var resultado = await service.ListarAsync(consulta);

        Assert.Equal(consulta, repository.UltimaConsulta);
        Assert.Equal(2, resultado.Pagina);
        Assert.Equal(3, resultado.TamanoPagina);
        Assert.Equal(7, resultado.Total);
        Assert.All(resultado.Items, item => Assert.IsType<ProveedorDto>(item));
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task ListarHistoricoAsync_DebeRetornarFechaDeBaja()
    {
        var instante = new DateTimeOffset(2026, 8, 15, 18, 0, 0, TimeSpan.Zero);
        var proveedor = Proveedor.Crear("Proveedor histórico");
        proveedor.DarDeBaja(instante);
        var repository = new RepositorioConsultaEnMemoria
        {
            PaginaHistorica = new PaginaProveedores([proveedor], 1)
        };
        var service = new ConsultarProveedorService(repository);

        var resultado = await service.ListarHistoricoAsync(new ConsultarProveedoresRequest());

        var item = Assert.Single(resultado.Items);
        Assert.Equal(proveedor.Id, item.Id);
        Assert.Equal(instante, item.DeletedAt);
    }

    private sealed class RepositorioConsultaEnMemoria : IProveedorConsultaRepository
    {
        public Proveedor? PorId { get; init; }
        public Proveedor? HistoricoPorId { get; init; }
        public PaginaProveedores Pagina { get; init; } = new([], 0);
        public PaginaProveedores PaginaHistorica { get; init; } = new([], 0);
        public ConsultarProveedoresRequest? UltimaConsulta { get; private set; }

        public Task<Proveedor?> ObtenerPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) => Task.FromResult(PorId);

        public Task<Proveedor?> ObtenerHistoricoPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) => Task.FromResult(HistoricoPorId);

        public Task<PaginaProveedores> ListarAsync(
            ConsultarProveedoresRequest consulta,
            CancellationToken cancellationToken = default)
        {
            UltimaConsulta = consulta;
            return Task.FromResult(Pagina);
        }

        public Task<PaginaProveedores> ListarHistoricoAsync(
            ConsultarProveedoresRequest consulta,
            CancellationToken cancellationToken = default)
        {
            UltimaConsulta = consulta;
            return Task.FromResult(PaginaHistorica);
        }
    }
}
