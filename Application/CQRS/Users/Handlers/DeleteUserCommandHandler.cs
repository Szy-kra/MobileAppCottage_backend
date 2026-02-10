using MediatR;
using Microsoft.AspNetCore.Identity;
using MobileAppCottage.Domain.Entities;

namespace MobileAppCottage.Application.CQRS.Users.Handlers;

public record DeleteUserCommand(string UserId) : IRequest;

public class DeleteUserCommandHandler(UserManager<User> userManager) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null) return;

        // Tutaj w przyszłości możesz dodać:
        // _fileService.DeleteAllUserImages(user.Id);

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception("Nie udało się usunąć użytkownika");
        }
    }
}