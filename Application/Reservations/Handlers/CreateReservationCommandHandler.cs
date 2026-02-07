using MediatR;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Application.Reservations.Commands;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Infrastructure.Persistence;

namespace MobileAppCottage.Application.Reservations.Handlers
{
    // KLUCZOWE: Musi być <CreateReservationCommand, int> a nie samo <CreateReservationCommand>
    public class CreateReservationCommandHandler : IRequestHandler<CreateReservationCommand, int>
    {
        private readonly CottageDbContext _context;

        public CreateReservationCommandHandler(CottageDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateReservationCommand request, CancellationToken cancellationToken)
        {
            // Sprawdzenie czy termin jest wolny
            var isOverlapping = await _context.CottageReservations
                .AnyAsync(r => r.CottageId == request.CottageId &&
                               request.From < r.To &&
                               r.From < request.To, cancellationToken);

            if (isOverlapping)
            {
                throw new Exception("Wybrany termin jest już zajęty.");
            }

            var reservation = new CottageReservation
            {
                CottageId = request.CottageId,
                From = request.From,
                To = request.To,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                ReservedById = request.ReservedById,
                IsForSomeoneElse = request.IsForSomeoneElse,
                IsPaid = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.CottageReservations.Add(reservation);
            await _context.SaveChangesAsync(cancellationToken);

            // Zwracamy int, bo tego oczekuje IRequestHandler i Twoja komenda
            return reservation.Id;
        }
    }
}