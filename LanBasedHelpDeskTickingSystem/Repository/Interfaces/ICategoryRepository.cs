using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface ICategoryRepository
{
    public Task<IEnumerable<Category>> GetAllCategoriesAsync();
}