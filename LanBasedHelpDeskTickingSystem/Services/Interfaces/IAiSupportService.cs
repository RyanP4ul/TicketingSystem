namespace LanBasedHelpDeskTickingSystem.Services.Interfaces;

public interface IAiSupportService
{
    public Task<string?> GetAnswerFromArticleAsync(string prompt);
}