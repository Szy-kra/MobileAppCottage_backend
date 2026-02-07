using MobileAppCottage.Domain.Interfaces;
using System.Security.Claims;

namespace MobileAppCottage.Infrastructure.UserContext
{
    public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
    {
        public CurrentUser? GetCurrentUser()
        {
            var user = httpContextAccessor.HttpContext?.User;

            if (user?.Identity is null || !user.Identity.IsAuthenticated)
                return null;

            var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value;

            return (id is null || email is null) ? null : new CurrentUser(id, email);
        }
    }
}