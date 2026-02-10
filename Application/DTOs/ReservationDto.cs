namespace MobileAppCottage.Application.DTOs
{
    public class ReservationDto
    {
        public int Id { get; set; }

        // Data rozpoczęcia rezerwacji
        public DateTime StartDate { get; set; }

        // Data zakończenia rezerwacji (używana w Twoim Handlerze do filtrowania r.EndDate < now)
        public DateTime EndDate { get; set; }

        public bool IsAvailable { get; set; } = false;

        // Dodatkowe przydatne pole, jeśli chcesz wyświetlić nazwę domku w profilu
        public string? CottageName { get; set; }
    }
}