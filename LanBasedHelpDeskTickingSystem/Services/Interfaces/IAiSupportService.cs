using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Services.Interfaces;

public interface IAiSupportService
{
    
    public Task<string?> GetAnswerFromArticleAsync(string prompt);
    public Task<List<KnowledgeMatchResponse>> GetTicketSuggestionsAsync(string ticketTitle, string ticketDescription);
    public Task<string> AnalyzeTicketPriorityAsync(string title, string description);
    public Task<bool> IsTicketRelevantAsync(string title, string description);
}