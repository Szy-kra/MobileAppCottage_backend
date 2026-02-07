using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileAppCottage.Domain.Entities
{
    public class CottageReservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CottageId { get; set; }

        [ForeignKey("CottageId")]
        public virtual Cottage? Cottage { get; set; }

        // ZMIANA: Nazwy zgodne z ReservationDto dla kalendarza
        [Required(ErrorMessage = "Data rozpoczęcia jest wymagana.")]
        public DateTime From { get; set; }

        [Required(ErrorMessage = "Data zakończenia jest wymagana.")]
        public DateTime To { get; set; }

        [Required(ErrorMessage = "Nazwa klienta nie może być pusta.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
        [Phone(ErrorMessage = "Niepoprawny format numeru telefonu.")]
        public string CustomerPhone { get; set; } = string.Empty;

        public bool IsPaid { get; set; } = false;

        // ZMIANA: UtcNow dla spójności czasowej [cite: 2026-01-12]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ReservedById { get; set; }

        [ForeignKey("ReservedById")]
        public virtual User? ReservedBy { get; set; }

        public bool IsForSomeoneElse { get; set; } = false;
    }
}