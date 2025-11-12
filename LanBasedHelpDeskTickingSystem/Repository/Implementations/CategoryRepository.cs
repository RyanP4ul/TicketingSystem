using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class CategoryRepository(AppDbContext db) : ICategoryRepository
{
    
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await db.SetEntity<Category>().ToListAsync();
    }
    
}