namespace LanBasedHelpDeskTickingSystem.Services.Interfaces;

public interface IFileService
{
    public Task<KeyValuePair<string, string>?> SaveFileAsync(IFormFile file);
}