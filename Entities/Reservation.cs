using System.ComponentModel.DataAnnotations;

namespace CottageAPI.Entities
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal PricePerNightSnapshot { get; set; }

        public decimal TotalPrice { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Relacja do domku zostaje
        public int CottageId { get; set; }
        public virtual Cottage Cottage { get; set; } = null!;

        // USUNIĘTO IdentityUser Client! 
        // Jeśli potrzebujesz wiedzieć czyja to rezerwacja, na razie dodaj prosty string lub int:
        public string? ClientName { get; set; }
    }

    public enum ReservationStatus
    {
        Pending = 0,    // Oczekuje
        Confirmed = 1,  // Potwierdzone
        Cancelled = 2,  // Anulowane
        Completed = 3   // Zakonczone
    }
}