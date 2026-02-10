using MediatR;

namespace MobileAppCottage.Application.CQRS.Reservations.Commands
{
    public class UpdateReservationByHostCommand : IRequest
    {
        public int Id { get; set; }

        // Host może zmienić zakres dat
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        // Host może zmienić status płatności i anulowania
        public bool IsPaid { get; set; }
        public bool IsCancelled { get; set; }

        // Opcjonalna notatka, np. "Przełożono na prośbę klienta"
        public string? HostNotes { get; set; }
    }
}