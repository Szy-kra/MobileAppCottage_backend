namespace MobileAppCottage.Infrastructure.IdentityContext
{

    public class CurrentUser(string id, string email)
    {
        public string Id { get; set; } = id;
        public string Email { get; set; } = email;
    }
}
