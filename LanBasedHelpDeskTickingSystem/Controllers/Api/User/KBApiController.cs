using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Libs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api.User;

[Authorize(Roles = nameof(UserRole.User))]
[Route("api/user/kb")]
[ApiController]
public class KBApiController(AppDbContext db) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> ListKnowledgeBase([FromQuery] string? search, [FromQuery] string? category, [FromQuery] int limit = 2, [FromQuery] int page = 1)
    {
        if (limit < 1) limit = 1;
        if (page < 1) page = 1;
        
        var query = db.SetEntity<KnowledgeBase>()
            .Include(x => x.Category)
            .Include(x => x.Author)
            .AsNoTracking();
        
        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Title.Contains(search));
        if (!string.IsNullOrEmpty(category)) query = query.Where(t => t.Category != null && t.Category.Name == category);
        
        var kbQuery = query.OrderBy(t => t.Id);
        var paginatedKbs = await PaginatedList<KnowledgeBase>.CreateAsync(kbQuery, page, limit);
        
        return Ok(PaginateResponse<KnowledgeBase>.Create(paginatedKbs));
    }
    
}