using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class KnowledgeBaseRepository(AppDbContext db) : IKnowledgeBaseRepository
{
    
    public async Task<int> GetTotalArticlesAsync()
    {
        return await db.SetEntity<KnowledgeBase>().CountAsync(x => !x.IsDeleted);
    }
    
    public async Task<int> GetTotalPublishedArticlesAsync()
    {
        return await db.SetEntity<KnowledgeBase>().CountAsync(kb => kb.IsPublished && !kb.IsDeleted);
    }
    
    public async Task<int> GetTotalDraftsArticlesAsync()
    {
        return await db.SetEntity<KnowledgeBase>().CountAsync(kb => !kb.IsPublished && !kb.IsDeleted);
    }
    
    public async Task<int> GetTotalViewArticleAsync()
    {
        return await db.SetEntity<KnowledgeBase>().SumAsync(x => x.ViewCount);
    }
    
    public async Task<KnowledgeBase?> GetArticleByIdAsync(int id)
    {
        return await db.SetEntity<KnowledgeBase>().Include(x => x.Category).FirstOrDefaultAsync(kb => kb.Id == id && !kb.IsDeleted);
    }
    
    public async Task<ApiResultResponse> InsertArticleAsync(string title, string content, int categoryId, int authorId, string tags)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var newKb = new KnowledgeBase
            {
                Title = title,
                Content = content,
                CategoryId = categoryId,
                AuthorId = authorId,
                IsPublished = false,
                Tags = tags
            };
            
            db.SetEntity<KnowledgeBase>().Add(newKb);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return ApiResultResponse.Ok("KB created successfully.");
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while creating.");
        }
    }
    
    public async Task<ApiResultResponse> UpdatePublishedStatus(int kbId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var existingArticle = await GetArticleByIdAsync(kbId);
        
            if (existingArticle == null)
            {
                return ApiResultResponse.Error("Knowledge base article not found");
            }

            existingArticle.IsPublished = !existingArticle.IsPublished;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return ApiResultResponse.Ok("KB published status updated successfully.");
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred.");
        }
    }

    public async Task<ApiResultResponse> UpdateArticle(int kbId, string title, string content, int catId, string tags)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var existingArticle = await GetArticleByIdAsync(kbId);
        
            if (existingArticle == null)
            {
                return ApiResultResponse.Error("Knowledge base article not found");
            }
            
            existingArticle.Title = title;
            existingArticle.Content = content;
            existingArticle.CategoryId = catId;
            existingArticle.Tags = tags;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return ApiResultResponse.Ok("KB updated successfully.");
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while creating.");
        }
    }

    public async Task<ApiResultResponse> DeleteArticleByIdAsync(int userId, int articleId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var knowledgeBase = await db.SetEntity<KnowledgeBase>().FirstOrDefaultAsync(kb => kb.Id == articleId);
            
            if (knowledgeBase == null)
            {
                return ApiResultResponse.Error("Knowledge base article not found");
            }

            knowledgeBase.IsDeleted = true;
            knowledgeBase.DeletedBy = userId;
            knowledgeBase.DeletedAt = DateTime.UtcNow;
            // db.SetEntity<KnowledgeBase>().Remove(existingArticle);
                
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return ApiResultResponse.Ok("KB deleted successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while deleting.");
        }
    }
    
}