namespace MobileAppCottage.Domain.Interfaces
{
    public interface IFileService
    {
        // Teraz przyjmuje stringi, więc SaveImage(base64, fileName) w handlerze zadziała
        Task<string> SaveImage(string base64String, string fileName);
        void DeleteFile(string path);
    }
}