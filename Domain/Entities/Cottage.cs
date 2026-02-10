using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileAppCottage.Domain.Entities
{
    public class Cottage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public string? About { get; set; }

        // Pola ContactDetails są zmapowane jako kolumny w tej samej tabeli
        public CottageDetails ContactDetails { get; set; } = new CottageDetails();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? EncodedName { get; set; }
        public string? OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public virtual User? Owner { get; set; }

        // --- POPRAWIONE: Zmieniono z ICottageImage na CottageImage ---
        // To powiązanie tworzy relację 1:N widoczną na diagramie
        public virtual List<CottageImage> Images { get; set; } = new List<CottageImage>();

        public virtual List<CottageReservation> Reservations { get; set; } = new List<CottageReservation>();

        public void EncodeName()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                EncodedName = Name.ToLower().Replace(" ", "-");
            }
        }
    }
}