using MediatR;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Application.Reservations.Commands;
// Zakładam, że Twój DbContext jest w tej przestrzeni:
// using MobileAppCottage.Infrastructure.Persistence; 

namespace MobileAppCottage.Application.Reservations.Handlers
{
    public class DeleteReservationCommandHandler : IRequestHandler<DeleteReservationCommand>
    {
        // Tutaj musisz wstrzyknąć swój DbContext (np. IApplicationDbContext lub CottageDbContext)
        private readonly DbContext _context;

        public DeleteReservationCommandHandler(DbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteReservationCommand request, CancellationToken cancellationToken)
        {
            // 1. Znajdź rezerwację w bazie
            var reservation = await _context.Set<Domain.Entities.CottageReservation>()
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            // 2. Jeśli nie istnieje, możesz rzucić wyjątek (opcjonalnie)
            if (reservation == null)
            {
                throw new Exception("Nie znaleziono rezerwacji o podanym ID.");
            }

            // 3. Usuń z bazy i zapisz zmiany
            _context.Set<Domain.Entities.CottageReservation>().Remove(reservation);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}