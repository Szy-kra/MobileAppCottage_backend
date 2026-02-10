using MediatR;
using MobileAppCottage.Application.CQRS.Reservations.Commands;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces; // Kluczowe dla błędu IUserContext
using MobileAppCottage.Infrastructure.Persistence;

namespace MobileAppCottage.Application.CQRS.Reservations.Handlers
{
    public class CreateReservationCommandHandler(
        CottageDbContext dbContext,
        IUserContext userContext) : IRequestHandler<CreateReservationCommand, int>
    {
        public async Task<int> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();

            // Jeśli użytkownik nie jest zalogowany, wyrzucamy błąd (aplikacja mobilna go obsłuży)
            if (currentUser == null)
                throw new UnauthorizedAccessException();

            var reservation = new CottageReservation
            {
                CottageId = request.CottageId,
                From = request.From,
                To = request.To,
                ReservedById = currentUser.Id, // Powiązanie z profilu z tokena
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                IsPaid = false,
                IsCancelled = false
            };

            dbContext.CottageReservations.Add(reservation);
            await dbContext.SaveChangesAsync(cancellationToken);

            return reservation.Id;
        }
    }
}