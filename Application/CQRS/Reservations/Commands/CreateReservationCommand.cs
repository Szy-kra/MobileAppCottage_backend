using MediatR;

namespace MobileAppCottage.Application.CQRS.Reservations.Commands
{
    public class CreateReservationCommand : IRequest<int>
    {
        public int CottageId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}