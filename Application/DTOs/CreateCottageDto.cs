using System.ComponentModel.DataAnnotations;

namespace MobileAppCottage.Application.DTOs
{
    public class CottageCreateDto
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public ContactDetailsCreateDto ContactDetails { get; set; } = new();
    }

    public class ContactDetailsCreateDto
    {
        [Range(1, 10000)]
        public decimal Price { get; set; }

        [Range(1, 50)]
        public int MaxPersons { get; set; }

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;
    }
}
