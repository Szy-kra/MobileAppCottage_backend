using MediatR;

namespace MobileAppCottage.Application.CQRS.Users.Commands
{
    public class UpdateUserProfileCommand : IRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}