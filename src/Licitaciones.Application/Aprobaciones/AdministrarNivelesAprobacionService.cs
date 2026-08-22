using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Aprobaciones;

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
}
