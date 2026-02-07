using MediatR;
using MobileAppCottage.Application.DTOs;

namespace MobileAppCottage.Application.Reservations.Queries
{
    // Zwracamy listę DTO, którą Twój frontend już potrafi obsłużyć
    public class GetCottageReservationsQuery : IRequest<List<ReservationDto>>
    {
        public int CottageId { get; }

        public GetCottageReservationsQuery(int cottageId)
        {
            CottageId = cottageId;
        }
    }
}