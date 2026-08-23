using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Aprobaciones;
using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Aprobaciones;

public sealed class AdministrarNivelesAprobacionService
{
    private readonly INivelAprobacionRepository _repository;

    public AdministrarNivelesAprobacionService(INivelAprobacionRepository repository) =>
        _repository = repository;

    public async Task<LicitacionNivelAprobacionDto> CrearAsync(
        string nombre,
        decimal montoMinimo,
        decimal? montoMaximo,
        CancellationToken cancellationToken = default)
    {
        if (await _repository.ExisteTraslapeActivoAsync(
                montoMinimo, montoMaximo, cancellationToken: cancellationToken))
        {
            throw new NivelAprobacionConflictoException();
        }

        var nivel = NivelAprobacion.Crear(nombre, montoMinimo, montoMaximo);
        await _repository.AgregarAsync(nivel, cancellationToken);
        return new LicitacionNivelAprobacionDto(nivel.Id, nivel.Nombre);
    }

    public async Task<PaginaResultado<NivelAprobacionResumenDto>> ListarAsync(
        NivelesAprobacionConsultaRequest consulta,
        CancellationToken cancellationToken = default)
    {
        ValidarConsulta(consulta);

        var niveles = await _repository.ListarActivosAsync(cancellationToken);

        IEnumerable<NivelAprobacion> filtrados = niveles;
        if (!string.IsNullOrWhiteSpace(consulta.Nombre))
        {
            filtrados = filtrados.Where(nivel => nivel.Nombre.Contains(
                consulta.Nombre.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        filtrados = (consulta.OrdenarPor.ToLowerInvariant(), consulta.Descendente) switch
        {
            ("nombre", false) => filtrados.OrderBy(nivel => nivel.Nombre),
            ("nombre", true) => filtrados.OrderByDescending(nivel => nivel.Nombre),
            ("montominimo", true) => filtrados.OrderByDescending(nivel => nivel.MontoMinimo),
            _ => filtrados.OrderBy(nivel => nivel.MontoMinimo)
        };

        var todos = filtrados.ToArray();
        var items = todos
            .Skip((consulta.Pagina - 1) * consulta.TamanoPagina)
            .Take(consulta.TamanoPagina)
            .Select(Mapear)
            .ToArray();

        return new PaginaResultado<NivelAprobacionResumenDto>(
            items, todos.Length, consulta.Pagina, consulta.TamanoPagina);
    }

    public async Task<NivelAprobacionResumenDto?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var nivel = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        return nivel is null ? null : Mapear(nivel);
    }

    public async Task<bool> DesactivarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var nivel = await _repository.ObtenerPorIdAsync(id, cancellationToken);

        if (nivel is null || !nivel.Activo)
        {
            return false;
        }

        nivel.Desactivar();
        await _repository.GuardarCambiosAsync(cancellationToken);
        return true;
    }

    private static NivelAprobacionResumenDto Mapear(NivelAprobacion nivel) => new(
        nivel.Id,
        nivel.Nombre,
        nivel.MontoMinimo,
        nivel.MontoMaximo,
        nivel.Activo);

    private static void ValidarConsulta(NivelesAprobacionConsultaRequest consulta)
    {
        if (consulta.Pagina <= 0 || consulta.TamanoPagina is <= 0 or > 100)
        {
            throw new DomainException("La paginación solicitada no es válida.");
        }

        if (consulta.OrdenarPor.ToLowerInvariant() is not ("montominimo" or "nombre"))
        {
            throw new DomainException("El campo de ordenamiento no es válido.");
        }
    }
}
