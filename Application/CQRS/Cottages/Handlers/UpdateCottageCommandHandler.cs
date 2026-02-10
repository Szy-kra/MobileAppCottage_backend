using AutoMapper;
using MediatR;
using MobileAppCottage.Application.CQRS.Cottages.Commands;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces;

namespace MobileAppCottage.Application.CQRS.Cottages.Handlers
{
    // Użycie Primary Constructor (nowość .NET 8) dla wstrzykiwania zależności
    public class UpdateCottageCommandHandler(
        ICottageRepository cottageRepository,
        IMapper mapper,
        IFileService fileService) : IRequestHandler<UpdateCottageCommand>
    {
        public async Task Handle(UpdateCottageCommand request, CancellationToken cancellationToken)
        {
            // 1. Pobieramy domek (repozytorium powinno ładować również Images i ContactDetails)
            var cottage = await cottageRepository.GetById(request.Id);

            if (cottage == null)
            {
                throw new Exception("Cottage not found");
            }

            // 2. Mapujemy dane z komendy do encji
            // Dzięki poprawionemu wcześniej MappingProfile, to zaktualizuje:
            // Name, Description (zajawka), About (pełny opis) oraz ContactDetails
            mapper.Map(request, cottage);

            // 3. Odświeżenie przyjaznego adresu URL (slug) po ewentualnej zmianie nazwy
            cottage.EncodeName();

            // 4. OBSŁUGA NOWYCH ZDJĘĆ
            if (request.NewImagesBase64 != null && request.NewImagesBase64.Any())
            {
                foreach (var base64 in request.NewImagesBase64)
                {
                    // Zapisujemy fizycznie na dysku/serwerze (np. wwwroot/images)
                    var fileName = $"cottage_{cottage.Id}_{Guid.NewGuid()}.jpg";
                    var imageUrl = await fileService.SaveImage(base64, fileName);

                    // Tworzymy nową encję zdjęcia
                    var newImage = new CottageImage
                    {
                        Url = imageUrl,
                        CottageId = cottage.Id
                    };

                    // Dodajemy rekord do tabeli CottageImages
                    await cottageRepository.AddImage(newImage);
                }
            }

            // 5. Zapisujemy zaktualizowaną encję Cottage
            await cottageRepository.Update(request.Id, cottage);
        }
    }
}