using MediatR;

namespace MobileAppCottage.Application.CQRS.Cottages.Commands
{
    // Zwracamy int (Id nowego domku)
    public class CreateCottageCommand : IRequest<int>
    {
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public string? City { get; set; }

        public string? Street { get; set; }
        public string? PostalCode { get; set; }
    }
}