using Microsoft.EntityFrameworkCore;
using MobileAppCottage.Domain.Entities;
using MobileAppCottage.Domain.Interfaces;
using MobileAppCottage.Infrastructure.Persistence;

namespace MobileAppCottage.Infrastructure.Repositories
{
    public class CottageRepository(CottageDbContext dbContext) : ICottageRepository
    {
        public async Task<int> Create(Cottage cottage)
        {
            dbContext.Cottages.Add(cottage);
            await dbContext.SaveChangesAsync();
            return cottage.Id;
        }

        public async Task<IEnumerable<Cottage>> GetAll()
            => await dbContext.Cottages
                .Include(c => c.Images)
                .ToListAsync();

        public async Task<Cottage?> GetById(int id)
            => await dbContext.Cottages
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Cottage?> GetByEncodedName(string encodedName)
            => await dbContext.Cottages
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.EncodedName == encodedName);

        public async Task Update(int id, Cottage cottage)
        {
            // W .NET 8, jeśli encja jest śledzona, wystarczy SaveChanges
            await dbContext.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var cottage = await dbContext.Cottages.FindAsync(id);
            if (cottage != null)
            {
                dbContext.Cottages.Remove(cottage);
                await dbContext.SaveChangesAsync();
            }
        }

        // --- Metody dla zdjęć ---
        public async Task AddImage(CottageImage image)
        {
            dbContext.CottageImages.Add(image);
            await dbContext.SaveChangesAsync();
        }

        public async Task<CottageImage?> GetImageById(int imageId)
        {
            return await dbContext.CottageImages
                .FirstOrDefaultAsync(i => i.Id == imageId);
        }

        public async Task DeleteImage(CottageImage image)
        {
            dbContext.CottageImages.Remove(image);
            await dbContext.SaveChangesAsync();
        }
    }
}