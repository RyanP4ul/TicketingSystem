using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;

namespace LanBasedHelpDeskTickingSystem.Services.Implementations;

public class FileService : IFileService
{
    
    public static readonly string[] AllowedExtensions = { "jpg", "jpeg", "png", "pdf" };
    
    public async Task<KeyValuePair<string, string>?> SaveFileAsync(IFormFile file)
    {
        try
        {
            long maxFileSize = 5 * 1024 * 1024;
            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads");

            if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

            var ext = Path.GetExtension(file.FileName).ToLower().TrimStart('.') ?? "";

            if (!AllowedExtensions.Contains(ext) || file.Length > maxFileSize) return null;

            var generateFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow.Ticks}";
            var distinctFileName = $"{generateFileName}.{ext}";
            var filePath = Path.Combine(uploadPath, distinctFileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return new KeyValuePair<string, string>(ext, generateFileName);
        }
        catch (Exception)
        {
            return null;
        }
    }
    
}