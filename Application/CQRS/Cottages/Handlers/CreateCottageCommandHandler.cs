using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore; // Potrzebne do .FirstOrDefaultAsync()
using MobileAppCottage.Application.CQRS.Cottages.Commands;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces;
using MobileAppCottage.Infrastructure.Persistence; // Twoja klasa Contextu

namespace MobileAppCottage.Application.CQRS.Cottages.Handlers
{
    public class CreateCottageCommandHandler : IRequestHandler<CreateCottageCommand, int>
    {
        private readonly ICottageRepository _cottageRepository;
        private readonly IMapper _mapper;
        private readonly CottageDbContext _context; // Dodajemy bezpośrednio context do testu

        public CreateCottageCommandHandler(ICottageRepository cottageRepository, IMapper mapper, CottageDbContext context)
        {
            _cottageRepository = cottageRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<int> Handle(CreateCottageCommand request, CancellationToken cancellationToken)
        {
            var cottage = _mapper.Map<Cottage>(request);

            // PRÓBA RATUNKU: Pobieramy ID pierwszego lepszego użytkownika z bazy
            // Jeśli baza jest pusta, to tu wybuchnie - i o to chodzi, będziemy wiedzieć!
            var firstUser = await _context.Users.FirstOrDefaultAsync();

            if (firstUser != null)
            {
                cottage.OwnerId = firstUser.Id;
            }
            else
            {
                // Jeśli nie ma użytkowników, wpisujemy cokolwiek, ale to może rzucić błąd
                // Wtedy będziesz wiedział, że musisz najpierw kogoś zarejestrować!
                cottage.OwnerId = Guid.NewGuid().ToString();
            }

            // Generujemy EncodedName (często wymagane przez logikę domenową)
            cottage.EncodedName = cottage.Name.ToLower().Replace(" ", "-");

            var id = await _cottageRepository.Create(cottage);

            return id;
        }
    }
}