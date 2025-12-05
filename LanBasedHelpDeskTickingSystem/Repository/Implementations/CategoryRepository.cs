using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await db.SetEntity<Category>().ToListAsync();
    }
    
    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await db.SetEntity<Category>().FirstOrDefaultAsync(c => c.Id == id);
    }
    
    public async Task<ApiResultResponse> InsertCategoryAsync(string name, string description)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        
        try
        {
            var errors = new Dictionary<string, string[]>();
            var hasName = await db.SetEntity<Category>().CountAsync(x => x.Name == name) > 0;

            if (hasName)
            {
                errors.Add("Name", new[] { "Category with the same name already exists." });
            }
            
            if (errors.Count > 0)
            {
                return ApiResultResponse.Error(errors);
            }
            
            var category = new Category
            {
                Name = name,
                Description = description
            };
        
            await db.SetEntity<Category>().AddAsync(category);
            await db.SaveChangesAsync();
            
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Category created successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while creating.");
        }
    }

    public async Task<ApiResultResponse> UpdateCategoryAsync(int id, string name, string description)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        
        try
        {
            var errors = new Dictionary<string, string[]>();
            var existCategory = await db.SetEntity<Category>().FirstOrDefaultAsync(x => x.Id == id);

            if (existCategory == null)
            {
                return ApiResultResponse.Error("Category not found.");
            }
            
            var hasName = await db.SetEntity<Category>().CountAsync(x => x.Name == name && x.Name != existCategory.Name) > 0;

            if (hasName)
            {
                errors.Add("Name", new[] { "Category with the same name already exists." });
            }
            
            if (errors.Count > 0)
            {
                return ApiResultResponse.Error(errors);
            }
            
            existCategory.Name = name;
            existCategory.Description = description;
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Category updated successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while updating.");
        }
    }

    public async Task<ApiResultResponse> DeleteCategoryByIdAsync(int id)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        
        try
        {
            var existCategory = await db.SetEntity<Category>().FirstOrDefaultAsync(x => x.Id == id);

            if (existCategory == null)
            {
                return ApiResultResponse.Error("Category not found.");
            }
            
            db.SetEntity<Category>().Remove(existCategory);
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Category deleted successfully.");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while deleting.");
        }
    }
    
    public async Task<IEnumerable<Category>> GetPopularCategoriesAsync()
    {
        return await db.SetEntity<Ticket>()
            .Where(x => !x.IsDeleted)
            .GroupBy(t => t.CategoryId)
            .OrderByDescending(g => g.Count())
            .Take(5)
            .Select(g => new Category
            {
                Id = g.Key,
                Name = db.SetEntity<Category>().Where(c => c.Id == g.Key).Select(c => c.Name).FirstOrDefault() ?? string.Empty,
                Description = db.SetEntity<Category>().Where(c => c.Id == g.Key).Select(c => c.Description).FirstOrDefault() ?? string.Empty,
                Count = g.Count()
            })
            .ToListAsync();
    }
    
}