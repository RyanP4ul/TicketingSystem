using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IKnowledgeBaseRepository
{
    public Task<int> GetTotalArticlesAsync();
    public Task<int> GetTotalPublishedArticlesAsync();
    public Task<int> GetTotalViewArticleAsync();
    public Task<KnowledgeBase?> GetArticleByIdAsync(int id);
    public Task InsertArticleAsync(string title, string content, int categoryId, int authorId, string tags);
    public Task UpdatePublishedStatus(int kbId);
    public Task UpdateArticle(int kbId, string title, string content, int catId, string tags);
}