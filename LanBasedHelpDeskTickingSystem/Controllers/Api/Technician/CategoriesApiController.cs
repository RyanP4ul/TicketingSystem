using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Libs;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api.Technician;

[Authorize(Roles = nameof(UserRole.Technician))]
[Route("api/technician/categories")]
[ApiController]
public class CategoriesApiController(AppDbContext db, ICategoryRepository categoryRepository) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> ListCategories([FromQuery] string? search, [FromQuery] int limit = 2, [FromQuery] int page = 1)
    {
        if (limit < 1) limit = 1;
        if (page < 1) page = 1;
        
        var query = db.SetEntity<Category>().AsNoTracking();
        
        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Name.Contains(search));

        var kbQuery = query.OrderBy(t => t.Id);
        var paginatedKbs = await PaginatedList<Category>.CreateAsync(kbQuery, page, limit);
        
        return Ok(PaginateResponse<Category>.Create(paginatedKbs));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory([FromRoute] int id)
    {
        var category = await categoryRepository.GetCategoryByIdAsync(id);
        
        if (category == null)
        {
            return NotFound(ApiResponse<string>.Error("Category not found."));
        }

        return Ok(ApiResponse<Category>.Ok(category));
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryDto model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );
        
            return BadRequest(new { success = false, errors });
        }
        
        var response = await categoryRepository.UpdateCategoryAsync(model.id, model.Name, model.Description);
        
        if (!response.Success) return BadRequest(response);

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDto model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }

        var response = await categoryRepository.InsertCategoryAsync(model.Name, model.Description);
        
        if (!response.Success) return BadRequest(response);

        return Ok(response);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory([FromRoute] int id)
    {
        var response = await categoryRepository.DeleteCategoryByIdAsync(id);
        
        if (!response.Success) return BadRequest(response);

        return Ok(response);
    }
    
}