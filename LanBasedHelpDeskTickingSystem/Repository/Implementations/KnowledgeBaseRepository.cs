using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class KnowledgeBaseRepository(AppDbContext db) : IKnowledgeBaseRepository
{
    
    public async Task<int> GetTotalArticlesAsync()
    {
        return await db.SetEntity<KnowledgeBase>().CountAsync();
    }
    
    public async Task<int> GetTotalPublishedArticlesAsync()
    {
        return await db.SetEntity<KnowledgeBase>().CountAsync(kb => kb.IsPublished);
    }
    
    public async Task<int> GetTotalViewArticleAsync()
    {
        return await db.SetEntity<KnowledgeBase>().SumAsync(x => x.ViewCount);
    }
    
    public async Task<KnowledgeBase?> GetArticleByIdAsync(int id)
    {
        return await db.SetEntity<KnowledgeBase>().Include(x => x.Category).FirstOrDefaultAsync(kb => kb.Id == id);
    }
    
    public async Task InsertArticleAsync(string title, string content, int categoryId, int authorId, string tags)
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
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
        }
    }
    
    public async Task UpdatePublishedStatus(int kbId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var existingArticle = await GetArticleByIdAsync(kbId);
        
            if (existingArticle == null)
            {
                throw new Exception("Knowledge base article not found");
            }

            existingArticle.IsPublished = !existingArticle.IsPublished;

            await db.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
        }
    }

    public async Task UpdateArticle(int kbId, string title, string content, int catId, string tags)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var existingArticle = await GetArticleByIdAsync(kbId);
        
            if (existingArticle == null)
            {
                throw new Exception("Knowledge base article not found");
            }

            existingArticle.Title = title;
            existingArticle.Content = content;
            existingArticle.CategoryId = catId;
            existingArticle.Tags = tags;

            await db.SaveChangesAsync();
            
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
        }
    }
    
}