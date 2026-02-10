

namespace MobileAppCottage.Application.DTOs
{
    public class UserProfileDto
    {
        // Podstawowe dane
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsHost { get; set; }

        // --- TO MUSISZ DOPISAĆ ---

        // Lista domków (tylko dla Hosta)
        public List<CottageDto> MyCottages { get; set; } = new List<CottageDto>();

        // Rezerwacje (nadchodzące)
        public List<ReservationDto> ActiveReservations { get; set; } = new List<ReservationDto>();

        // Rezerwacje (stare/zakończone)
        public List<ReservationDto> PastReservations { get; set; } = new List<ReservationDto>();
    }
}