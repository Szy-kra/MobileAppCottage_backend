namespace CottageAPI.Entities
{
    public class Cottage
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int MaxPersons { get; set; }
        public string? About { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // TWOJE POLA ADRESOWE (Dopisane do klasy!) [cite: 2026-02-21]
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;


        public virtual List<CottageImage> Images { get; set; } = new();
        public virtual List<Reservation> Reservations { get; set; } = new();
    }
}