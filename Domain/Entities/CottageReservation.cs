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

        [Required]
        public DateTime From { get; set; }

        [Required]
        public DateTime To { get; set; }

        public string Status { get; set; } = "Pending"; // DODANO: Naprawia CreateReservationHandler

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string CustomerPhone { get; set; } = string.Empty;

        public bool IsPaid { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ReservedById { get; set; }
        [ForeignKey("ReservedById")]
        public virtual User? ReservedBy { get; set; }

        public string? HostNotes { get; set; }
    }
}