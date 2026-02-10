using MediatR;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Application.CQRS.Reservations.Commands;
using MobileAppCottage.Domain.Interfaces;
using MobileAppCottage.Infrastructure.Persistence;

namespace MobileAppCottage.Application.CQRS.Reservations.Handlers
{
    public class DeleteReservationCommandHandler(
        CottageDbContext dbContext,
        IUserContext userContext) : IRequestHandler<DeleteReservationCommand>
    {
        public async Task Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();

            // Zabezpieczenie przed nullem, jeśli użytkownik nie jest zalogowany
            if (currentUser == null) throw new UnauthorizedAccessException();

            var reservation = await dbContext.CottageReservations
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (reservation == null) return;

            // POPRAWKA: Używamy 'IsHost' z Twojego CurrentUser oraz 'To' z encji CottageReservation
            bool isHost = currentUser.IsHost;
            bool isPastReservation = reservation.To < DateTime.UtcNow;

            // ZASADA: Host może wszystko, Guest tylko swoje ZAKOŃCZONE rezerwacje
            if (!isHost)
            {
                // Dodatkowe sprawdzenie bezpieczeństwa: czy gość usuwa własną rezerwację?
                if (reservation.ReservedById != currentUser.Id)
                {
                    throw new UnauthorizedAccessException("Nie masz uprawnień do usunięcia tej rezerwacji.");
                }

                if (!isPastReservation)
                {
                    throw new InvalidOperationException("Gość może usuwać z widoku tylko zakończone rezerwacje.");
                }
            }

            dbContext.CottageReservations.Remove(reservation);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}