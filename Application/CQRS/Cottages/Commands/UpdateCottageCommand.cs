using MediatR;

namespace MobileAppCottage.Application.CQRS.Cottages.Commands
{
    public class UpdateCottageCommand : IRequest
    {
        public int Id { get; set; }

        // --- Dane główne encji Cottage ---

        // Nazwa domku wyświetlana wszędzie
        public string Name { get; set; } = default!;

        // "Zajawka" - krótki opis na HomeScreen / Listę domków
        public string Description { get; set; } = default!;

        // Pełny opis "O nas / O domku" na stronę szczegółów (bogaty tekst)
        public string? About { get; set; }

        // --- Dane z CottageDetails (Owned Type) ---

        public decimal? Price { get; set; }
        public int? MaxPersons { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }

        // --- Dane dla zdjęć z React Native ---

        // Lista nowych zdjęć przesyłanych jako Base64 (wymaga System.Collections.Generic)
        public List<string>? NewImagesBase64 { get; set; }
    }
}