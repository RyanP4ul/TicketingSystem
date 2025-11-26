using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Libs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api.Admin;

[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/admin/users")]
[ApiController]
public class UserApiController(AppDbContext db) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> ListUsers([FromQuery] string? search, [FromQuery] string? role, [FromQuery] int limit = 2, [FromQuery] int page = 1)
    {
        if (limit < 1) limit = 1;
        if (page < 1) page = 1;
        
        var query = db.SetEntity<Entities.Models.User>().Select(x => new
        {
            x.Id,
            x.Username,
            x.Email,
            Roles = x.Roles.ToString(),
            x.CreatedAt
        }).AsNoTracking();

        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Username.Contains(search));
        if (!string.IsNullOrEmpty(role) && Enum.TryParse<UserRole>(role, out var userRole)) query = query.Where(t => t.Roles.Contains(userRole.ToString()));
        
        var ticketsQuery = query.OrderBy(t => t.Id);
        var paginatedTickets = await PaginatedList<object>.CreateAsync(ticketsQuery, page, limit);
        
        return Ok(PaginateResponse<object>.Create(paginatedTickets));
    }
    
}