using MobileAppCottage.Domain.Interfaces;
using System.Security.Claims;

namespace MobileAppCottage.Infrastructure.UserContext
{
    // Upewnij się, że klasa CurrentUser jest widoczna w tym namespace
    public class IdentityContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity is null || !user.Identity.IsAuthenticated)
                return null;

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;

            // Pobieramy informację o tym, czy użytkownik jest hostem
            var isHost = user.IsInRole("Host") || user.FindFirst("IsHost")?.Value == "true";

            if (id is null || email is null)
                return null;

            // Zwracamy obiekt – upewnij się, że klasa CurrentUser ma te 3 pola w konstruktorze
            return new CurrentUser(id, email, isHost);
        }
    }
}