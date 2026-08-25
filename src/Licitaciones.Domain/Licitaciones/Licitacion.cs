using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Licitaciones;

public sealed class Licitacion : IAuditableEntity
{
    private readonly List<LicitacionTransicion> _transiciones = [];

    private Licitacion()
    {
    }

    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string CodigoNormalizado { get; private set; } = string.Empty;
    public string Titulo { get; private set; } = string.Empty;
    public decimal Presupuesto { get; private set; }
    public DateTimeOffset FechaCierre { get; private set; }
    public EstadoLicitacion Estado { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public uint Version { get; private set; }
    public IReadOnlyCollection<LicitacionTransicion> Transiciones => _transiciones.AsReadOnly();

    public static Licitacion Crear(
        string codigo,
        string titulo,
        decimal presupuesto,
        DateTimeOffset fechaCierre)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new DomainException("El código de la licitación es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new DomainException("El título de la licitación es obligatorio.");
        }

        if (presupuesto <= 0)
        {
            throw new DomainException("El presupuesto debe ser mayor que cero.");
        }

        return new Licitacion
        {
            Id = Guid.NewGuid(),
            Codigo = codigo.Trim(),
            CodigoNormalizado = codigo.Trim().ToUpperInvariant(),
            Titulo = titulo.Trim(),
            Presupuesto = presupuesto,
            FechaCierre = fechaCierre.ToUniversalTime(),
            Estado = EstadoLicitacion.Borrador
        };
    }

    public bool EstaVencida(IClock clock) => clock.UtcNow() >= FechaCierre;

    public bool EstaCerradaFormalmente() => Estado == EstadoLicitacion.Cerrada;

    public EstadoLicitacion EstadoEfectivo(IClock clock) =>
        Estado == EstadoLicitacion.Publicada && EstaVencida(clock)
            ? EstadoLicitacion.Cerrada
            : Estado;

    public void Publicar(IClock clock)
    {
        if (Estado != EstadoLicitacion.Borrador)
        {
            throw new DomainException(
                $"No se puede publicar una licitación en estado {Estado}.");
        }

        if (EstaVencida(clock))
        {
            throw new DomainException(
                "No se puede publicar una licitación cuya fecha de cierre ya pasó.");
        }

        var estadoAnterior = Estado;
        Estado = EstadoLicitacion.Publicada;
        _transiciones.Add(LicitacionTransicion.Crear(
            Id,
            estadoAnterior,
            Estado,
            clock.UtcNow()));
    }

    public void Cerrar(IClock clock)
    {
        if (Estado != EstadoLicitacion.Publicada)
        {
            throw new DomainException(
                $"No se puede cerrar una licitaciÃ³n en estado {Estado}.");
        }

        var estadoAnterior = Estado;
        Estado = EstadoLicitacion.Cerrada;
        _transiciones.Add(LicitacionTransicion.Crear(
            Id,
            estadoAnterior,
            Estado,
            clock.UtcNow()));
    }

    public void DarDeBaja(DateTimeOffset momento)
    {
        DeletedAt = momento;
    }

    public void Editar(
        string codigo,
        string titulo,
        decimal presupuesto,
        DateTimeOffset fechaCierre,
        IClock clock)
    {
        if (EstaCerradaFormalmente() || EstaVencida(clock))
        {
            throw new DomainException(
                "No se puede editar una licitación cerrada (formal o funcionalmente).");
        }

        if (string.IsNullOrWhiteSpace(codigo))
        {
            throw new DomainException("El código de la licitación es obligatorio.");
        }

        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new DomainException("El título de la licitación es obligatorio.");
        }

        if (presupuesto <= 0)
        {
            throw new DomainException("El presupuesto debe ser mayor que cero.");
        }

        Codigo = codigo.Trim();
        CodigoNormalizado = codigo.Trim().ToUpperInvariant();
        Titulo = titulo.Trim();
        Presupuesto = presupuesto;
        FechaCierre = fechaCierre.ToUniversalTime();
    }
}
