using MediatR;

namespace MobileAppCottage.Application.Reservations.Commands
{
    // IRequest<int> oznacza, że po stworzeniu rezerwacji serwer zwróci nam jej numer ID
    public class CreateReservationCommand : IRequest<int>
    {
        public int CottageId { get; set; }

        // Używamy From/To, żeby było spójne z Twoim DTO i kalendarzem
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        // Dane osoby przyjeżdżającej (Customer)
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        // Dane techniczne dla Identity
        public string? ReservedById { get; set; }
        public bool IsForSomeoneElse { get; set; } = false;
    }
}