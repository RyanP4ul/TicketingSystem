using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface ICategoryRepository
{
    public Task<IEnumerable<Category>> GetAllCategoriesAsync();
    public Task<Category?> GetCategoryByIdAsync(int id);
    public Task<ApiResultResponse> InsertCategoryAsync(string name, string description);
    public Task<ApiResultResponse> UpdateCategoryAsync(int id, string name, string description);
    public Task<ApiResultResponse> DeleteCategoryByIdAsync(int id);
}