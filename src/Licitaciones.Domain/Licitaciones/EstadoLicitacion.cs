namespace Licitaciones.Domain.Licitaciones;

/// <summary>
/// Define los posibles estados en el ciclo de vida de una licitación.
/// </summary>
public enum EstadoLicitacion
{
    /// <summary>Licitación en edición, aún no publicada.</summary>
    Borrador = 1,

    /// <summary>Licitación publicada y abierta para recepción de ofertas.</summary>
    Publicada = 2,

    /// <summary>Licitación cerrada formalmente al expirar el plazo.</summary>
    Cerrada = 3,

    /// <summary>Licitación adjudicada a un proveedor.</summary>
    Adjudicada = 4,

    /// <summary>Licitación cancelada por la institución.</summary>
    Cancelada = 5
}
