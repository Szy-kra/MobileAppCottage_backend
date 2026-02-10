using MediatR;
using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Application.CQRS.Users.Commands;
using MobileAppCottage.Domain.Interfaces;
using MobileAppCottage.Infrastructure.Persistence;

namespace MobileAppCottage.Application.CQRS.Users.Handlers
{
    public class UpdateUserProfileCommandHandler(
        CottageDbContext dbContext,
        IUserContext userContext) : IRequestHandler<UpdateUserProfileCommand>
    {
        public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            var currentUser = userContext.GetCurrentUser();
            if (currentUser == null) throw new UnauthorizedAccessException();

            // Szukamy użytkownika w bazie na podstawie ID z tokena
            var user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken);

            if (user == null) return;

            // Zostawiamy tylko podstawowe dane osobowe
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}