using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IKnowledgeBaseRepository
{
    public Task<int> GetTotalArticlesAsync();
    public Task<int> GetTotalPublishedArticlesAsync();
    public Task<int> GetTotalDraftsArticlesAsync();
    public Task<int> GetTotalViewArticleAsync();
    public Task<KnowledgeBase?> GetArticleByIdAsync(int id);
    public Task<IEnumerable<KnowledgeBase>> GetPopularArticlesAsync();
    public Task<ApiResultResponse> InsertArticleAsync(string title, string content, int categoryId, int authorId, string tags);
    public Task<ApiResultResponse> UpdatePublishedStatus(int kbId);
    public Task<ApiResultResponse> UpdateArticle(int kbId, string title, string content, int catId, string tags);
    public Task<ApiResultResponse> DeleteArticleByIdAsync(int userId, int articleId);
}