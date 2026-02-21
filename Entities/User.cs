using Microsoft.AspNetCore.Identity;

namespace CottageAPI.Entities
{
    // Ta klasa w SQL-u zmieni się w tabelę AspNetUsers [cite: 2026-01-12]
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}