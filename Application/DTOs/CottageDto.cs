using System.ComponentModel.DataAnnotations;

namespace MobileAppCottage.Application.DTOs
{
    public class CottageDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa jest wymagana")]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty; // Zamiast default! dla bezpieczeństwa

        public string? Description { get; set; }

        [Range(1, 10000, ErrorMessage = "Cena musi być dodatnia")]
        public decimal Price { get; set; }

        public int MaxPersons { get; set; }

        [Required(ErrorMessage = "Ulica jest wymagana")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Miasto jest wymagane")]
        public string City { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public string? About { get; set; }
        public string? EncodedName { get; set; }

        public List<string> ImageUrls { get; set; } = new List<string>();

        public List<ReservationDto> BookedDates { get; set; } = new List<ReservationDto>();
    }
}