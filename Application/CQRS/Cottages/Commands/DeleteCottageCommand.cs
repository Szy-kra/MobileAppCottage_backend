using MediatR;

namespace MobileAppCottage.Application.CQRS.Cottages.Commands
{
    public class DeleteCottageCommand : IRequest
    {
        public int Id { get; }

        public DeleteCottageCommand(int id)
        {
            Id = id;
        }
    }
}