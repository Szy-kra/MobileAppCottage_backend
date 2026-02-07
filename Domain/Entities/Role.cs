using Microsoft.AspNetCore.Identity;

namespace MobileAppCottage.Domain.Entities
{
    public class Role : IdentityRole
    {
        // Dodatkowe pole, które masz w tabeli SQL
        public string? Description { get; set; }
    }
}