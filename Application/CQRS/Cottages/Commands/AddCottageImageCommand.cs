using MediatR;

namespace MobileAppCottage.Application.CQRS.Cottages.Commands
{
    public class AddCottageImageCommand : IRequest<string>
    {
        public int CottageId { get; set; }

        // Zmieniono z File na Image, aby pasowało do Handlera
        public IFormFile Image { get; set; } = default!;
    }
}