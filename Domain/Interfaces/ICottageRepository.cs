using MobileAppCottage.Domain.Entities;

namespace MobileAppCottage.Domain.Interfaces
{
    public interface ICottageRepository
    {
        Task<int> Create(Cottage cottage);
        Task<IEnumerable<Cottage>> GetAll();

        // ZMIANA: Pobieranie domku musi teraz uwzględniać listę zdjęć
        Task<Cottage?> GetById(int id);
        Task<Cottage?> GetByEncodedName(string encodedName);

        Task Update(int id, Cottage cottage);
        Task Delete(int id);

        // --- NOWE METODY DLA ZDJĘĆ ---
        Task AddImage(CottageImage image);
        Task<CottageImage?> GetImageById(int imageId);
        Task DeleteImage(CottageImage image);
    }
}