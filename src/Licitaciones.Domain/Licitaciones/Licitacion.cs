using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Licitaciones;

public sealed class Licitacion : IAuditableEntity
{
    private Licitacion()
    {
    }

    public Guid Id { get; private set; }
    public string Codigo { get; private set; } = string.Empty;
    public string Titulo { get; private set; } = string.Empty;
    public decimal Presupuesto { get; private set; }
    public DateTimeOffset FechaCierre { get; private set; }
    public EstadoLicitacion Estado { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

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
            Titulo = titulo.Trim(),
            Presupuesto = presupuesto,
            FechaCierre = fechaCierre.ToUniversalTime(),
            Estado = EstadoLicitacion.Borrador
        };
    }

    public bool EstaVencida(IClock clock) => clock.UtcNow() >= FechaCierre;
}
