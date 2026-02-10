namespace MobileAppCottage.Domain.Interfaces
{
    public interface IUserContext
    {
        CurrentUser? GetCurrentUser();
    }

    // Rekord musi zawierać IsHost, bo używasz go w GetUserProfileQueryHandler
    public record CurrentUser(string Id, string Email, bool IsHost);
}