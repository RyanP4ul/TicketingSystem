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

namespace LanBasedHelpDeskTickingSystem.Controllers.Api.Admin;

[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/admin/kb")]
[ApiController]
public class KBApiController(AppDbContext db, IKnowledgeBaseRepository kbRepository) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> ListKnowledgeBase([FromQuery] string? search, [FromQuery] string? category,
        [FromQuery] string? date, [FromQuery] int limit = 2, [FromQuery] int page = 1)
    {
        if (limit < 1) limit = 1;
        if (page < 1) page = 1;

        var query = db.SetEntity<KnowledgeBase>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Title.Contains(search));
        if (!string.IsNullOrEmpty(category)) query = query.Where(t => t.Category != null && t.Category.Name == category);

        var kbQuery = query.Where(x => !x.IsDeleted).OrderBy(t => t.Id);
        var paginatedKbs = await PaginatedList<KnowledgeBase>.CreateAsync(kbQuery, page, limit);

        return Ok(PaginateResponse<KnowledgeBase>.Create(paginatedKbs));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetKbArticle([FromRoute] int id)
    {
        var article = await kbRepository.GetArticleByIdAsync(id);

        if (article == null)
        {
            return NotFound(ApiResponse<string>.Error("Knowledge base article not found"));
        }

        return Ok(ApiResponse<KnowledgeBase>.Ok(article));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateKbArticle([FromBody] AdminKbDto model)
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

        try
        {
            var response =
                await kbRepository.UpdateArticle(model.Id, model.Title, model.Content, model.CategoryId, model.Tags);

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse<string>.Error("Failed to update knowledge base article"));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateKbArticle([FromBody] AdminKbDto model)
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

        try
        {
            var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
            var response = await kbRepository.InsertArticleAsync(model.Title, model.Content, model.CategoryId, userId, model.Tags);
            
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest(ApiResultResponse.Error("Failed to create knowledge base article"));
        }
    }


    [HttpPut("publish/{id}")]
    public async Task<IActionResult> PublishArticle(int id)
    {
        try
        {
            var response = await kbRepository.UpdatePublishedStatus(id);

            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest(ApiResultResponse.Error("Failed to update published status"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKbArticle([FromRoute] int id)
    {
        try
        {
            var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
            var response = await kbRepository.DeleteArticleByIdAsync(userId, id);
            
            if (!response.Success) return BadRequest(response);

            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest(ApiResultResponse.Error("An error occurred while deleting."));
        }
    }
    
}

