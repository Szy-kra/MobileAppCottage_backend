using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Application.DTOs;
using MobileAppCottage.Application.Reservations.Queries;
using MobileAppCottage.Infrastructure.Persistence;

namespace MobileAppCottage.Application.Reservations.Handlers
{
    // KLUCZOWE: Typy w <...> muszą być identyczne jak w Query
    public class GetCottageReservationsQueryHandler : IRequestHandler<GetCottageReservationsQuery, List<ReservationDto>>
    {
        private readonly CottageDbContext _context;
        private readonly IMapper _mapper;

        public GetCottageReservationsQueryHandler(CottageDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Metoda musi zwracać Task<List<ReservationDto>>
        public async Task<List<ReservationDto>> Handle(GetCottageReservationsQuery request, CancellationToken cancellationToken)
        {
            // Pobieramy rezerwacje z bazy danych
            var reservations = await _context.CottageReservations
                .Where(r => r.CottageId == request.CottageId)
                .ToListAsync(cancellationToken);

            // Mapujemy encje na listę DTO
            return _mapper.Map<List<ReservationDto>>(reservations);
        }
    }
}