using Microsoft.AspNetCore.Identity;

namespace MobileAppCottage.Domain.Entities
{
    // Rozszerzamy standardowego użytkownika
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // DODAJEMY TO: Musi pasować do kolumny w SQL
        // Ustawienie "= false" sprawi, że SQL nie dostanie NULLa przy rejestracji
        public bool IsHost { get; set; } = false;

        // Dane firmowe (opcjonalne)
        public string? CompanyName { get; set; }
        public string? TaxId { get; set; }

        // RELACJE
        // Jako Host/Owner
        public virtual ICollection<Cottage> OwnedCottages { get; set; } = new List<Cottage>();

        // Jako Guest (rezerwacje)
        public virtual ICollection<CottageReservation> Reservations { get; set; } = new List<CottageReservation>();
    }
}