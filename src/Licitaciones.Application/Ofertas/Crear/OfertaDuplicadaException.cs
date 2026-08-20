using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Ofertas.Crear;

public sealed class OfertaDuplicadaException : DomainException
{
    public OfertaDuplicadaException()
        : base("El proveedor ya tiene una oferta activa para esta licitacion.")
    {
    }
}
