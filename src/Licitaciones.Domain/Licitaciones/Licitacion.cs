using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Licitaciones;

/// <summary>
/// Entidad raíz que representa una licitación publicada por la institución.
/// Gestiona su ciclo de vida mediante un máquina de estados (Borrador → Publicada → Cerrada).
/// </summary>
public sealed class Licitacion : IAuditableEntity
{
    private readonly List<LicitacionTransicion> _transiciones = [];

    private Licitacion()
    {
    }

    /// <summary>
    /// Identificador único de la licitación.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Código de referencia de la licitación.
    /// </summary>
    public string Codigo { get; private set; } = string.Empty;

    /// <summary>
    /// Código normalizado en mayúsculas para búsquedas.
    /// </summary>
    public string CodigoNormalizado { get; private set; } = string.Empty;

    /// <summary>
    /// Título descriptivo de la licitación.
    /// </summary>
    public string Titulo { get; private set; } = string.Empty;

    /// <summary>
    /// Presupuesto asignado para la licitación.
    /// </summary>
    public decimal Presupuesto { get; private set; }

    /// <summary>
    /// Fecha límite para recibir ofertas.
    /// </summary>
    public DateTimeOffset FechaCierre { get; private set; }

    /// <summary>
    /// Estado actual de la licitación en su ciclo de vida.
    /// </summary>
    public EstadoLicitacion Estado { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Fecha de baja lógica de la licitación, o null si está activa.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>
    /// Número de versión para control de concurrencia optimista.
    /// </summary>
    public uint Version { get; private set; }

    /// <summary>
    /// Historial de transiciones de estado de esta licitación.
    /// </summary>
    public IReadOnlyCollection<LicitacionTransicion> Transiciones => _transiciones.AsReadOnly();

    /// <summary>
    /// Crea una nueva licitación en estado Borrador.
    /// </summary>
    /// <param name="codigo">Código de referencia de la licitación.</param>
    /// <param name="titulo">Título descriptivo.</param>
    /// <param name="presupuesto">Presupuesto asignado (debe ser mayor que cero).</param>
    /// <param name="fechaCierre">Fecha límite para recibir ofertas.</param>
    /// <returns>Nueva instancia de <see cref="Licitacion"/> en estado Borrador.</returns>
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

    /// <summary>
    /// Determina si la licitación está vencida según el reloj del sistema.
    /// </summary>
    /// <param name="clock">Reloj del sistema.</param>
    /// <returns>true si la fecha actual es igual o posterior a la fecha de cierre.</returns>
    public bool EstaVencida(IClock clock) => clock.UtcNow() >= FechaCierre;

    /// <summary>
    /// Determina si la licitación fue cerrada formalmente (estado Cerrada).
    /// </summary>
    public bool EstaCerradaFormalmente() => Estado == EstadoLicitacion.Cerrada;

    /// <summary>
    /// Obtiene el estado efectivo considerando vencimiento automático.
    /// Una licitación Publicada cuya fecha de cierre pasó se considera Cerrada.
    /// </summary>
    /// <param name="clock">Reloj del sistema.</param>
    /// <returns>Estado actual o Cerrada si está vencida.</returns>
    public EstadoLicitacion EstadoEfectivo(IClock clock) =>
        Estado == EstadoLicitacion.Publicada && EstaVencida(clock)
            ? EstadoLicitacion.Cerrada
            : Estado;

    /// <summary>
    /// Publica la licitación, cambiándola de Borrador a Publicada.
    /// </summary>
    /// <param name="clock">Reloj del sistema para registrar la transición.</param>
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

    /// <summary>
    /// Cierra formalmente la licitación, cambiándola de Publicada a Cerrada.
    /// </summary>
    /// <param name="clock">Reloj del sistema para registrar la transición.</param>
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

    /// <summary>
    /// Marca la licitación como eliminada lógicamente.
    /// </summary>
    /// <param name="momento">Fecha y hora de la baja.</param>
    public void DarDeBaja(DateTimeOffset momento)
    {
        DeletedAt = momento;
    }

    /// <summary>
    /// Actualiza los datos de la licitación. Solo permite edición si no está cerrada ni vencida.
    /// </summary>
    /// <param name="codigo">Nuevo código de referencia.</param>
    /// <param name="titulo">Nuevo título descriptivo.</param>
    /// <param name="presupuesto">Nuevo presupuesto (debe ser mayor que cero).</param>
    /// <param name="fechaCierre">Nueva fecha límite de cierre.</param>
    /// <param name="clock">Reloj del sistema para validar vencimiento.</param>
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
