using MediatR;
using MobileAppCottage.Application.DTOs;

namespace MobileAppCottage.Application.CQRS.Cottages.Queries
{
    // Proste zapytanie o listę wszystkich domków
    public record GetAllCottagesQuery : IRequest<IEnumerable<CottageDto>>;
}