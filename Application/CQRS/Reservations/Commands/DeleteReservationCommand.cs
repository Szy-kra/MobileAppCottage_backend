using MediatR;

namespace MobileAppCottage.Application.CQRS.Reservations.Commands
{
    // Używamy "Primary Constructor" dostępnego w .NET 8 dla zwięzłości
    public class DeleteReservationCommand(int id) : IRequest
    {
        public int Id { get; } = id;
    }
}