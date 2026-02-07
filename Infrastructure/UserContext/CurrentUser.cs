namespace MobileAppCottage.Infrastructure.UserContext
{

    public class CurrentUser(string id, string email)
    {
        public string Id { get; set; } = id;
        public string Email { get; set; } = email;
    }
}