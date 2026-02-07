using MobileAppCottage.Infrastructure.UserContext; // <--- To naprawi błąd CS0246

namespace MobileAppCottage.Domain.Interfaces
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }
}