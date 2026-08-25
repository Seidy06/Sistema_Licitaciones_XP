using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Aprobaciones;
using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Aprobaciones;

/// <summary>
/// Servicio para administrar niveles de aprobación: crear, listar, actualizar y desactivar.
/// </summary>
public sealed class AdministrarNivelesAprobacionService
{
    private readonly INivelAprobacionRepository _repository;

    public AdministrarNivelesAprobacionService(INivelAprobacionRepository repository) =>
        _repository = repository;

    /// <summary>
    /// Crea un nuevo nivel de aprobación validando traslapes con niveles existentes.
    /// </summary>
    /// <param name="nombre">Nombre del nivel de aprobación.</param>
    /// <param name="montoMinimo">Monto mínimo del rango.</param>
    /// <param name="montoMaximo">Monto máximo del rango (null para ilimitado).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos del nivel creado.</returns>
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

    /// <summary>
    /// Lista niveles de aprobación activos con filtrado y paginación.
    /// </summary>
    /// <param name="consulta">Parámetros de filtrado y paginación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de resultados con los niveles encontrados.</returns>
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

    /// <summary>
    /// Obtiene un nivel de aprobación por su identificador.
    /// </summary>
    /// <param name="id">Identificador del nivel.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del nivel o null si no existe.</returns>
    public async Task<NivelAprobacionResumenDto?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var nivel = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        return nivel is null ? null : Mapear(nivel);
    }

    /// <summary>
    /// Actualiza un nivel de aprobación existente validando traslapes.
    /// </summary>
    /// <param name="id">Identificador del nivel a actualizar.</param>
    /// <param name="nombre">Nuevo nombre del nivel.</param>
    /// <param name="montoMinimo">Nuevo monto mínimo.</param>
    /// <param name="montoMaximo">Nuevo monto máximo (null para ilimitado).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del nivel actualizado o null si no existe.</returns>
    public async Task<NivelAprobacionResumenDto?> ActualizarAsync(
        int id,
        string nombre,
        decimal montoMinimo,
        decimal? montoMaximo,
        CancellationToken cancellationToken = default)
    {
        var nivel = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        if (nivel is null)
        {
            return null;
        }

        if (await _repository.ExisteTraslapeActivoAsync(
                montoMinimo, montoMaximo, id, cancellationToken))
        {
            throw new NivelAprobacionConflictoException();
        }

        nivel.Actualizar(nombre, montoMinimo, montoMaximo);
        await _repository.GuardarCambiosAsync(cancellationToken);
        return Mapear(nivel);
    }

    /// <summary>
    /// Desactiva un nivel de aprobación activo.
    /// </summary>
    /// <param name="id">Identificador del nivel a desactivar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>True si se desactivó, false si no existía o ya estaba inactivo.</returns>
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
