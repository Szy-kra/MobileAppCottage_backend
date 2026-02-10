namespace MobileAppCottage.Domain.Entities
{
    // Usunięto 'I' z nazwy klasy, bo to jest Encja (obiekt bazy), a nie interfejs
    public class CottageImage
    {
        public int Id { get; set; }
        public string Url { get; set; } = default!;

        public int CottageId { get; set; }

        public virtual Cottage? Cottage { get; set; }
    }
}