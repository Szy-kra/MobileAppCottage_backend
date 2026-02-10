using MobileAppCottage.Domain.Interfaces;

namespace MobileAppCottage.Infrastructure.Services
{
    public class FileService : IFileService
    {
        public async Task<string> SaveImage(string base64String, string fileName)
        {
            if (string.IsNullOrEmpty(base64String)) return string.Empty;

            // Ścieżka do folderu na serwerze
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            // Konwersja Base64 na bajty i zapis
            var imageBytes = Convert.FromBase64String(base64String);
            await File.WriteAllBytesAsync(filePath, imageBytes);

            return $"/images/{fileName}"; // Zwracamy relatywny URL do bazy
        }

        public void DeleteFile(string path)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", path.TrimStart('/'));
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
}