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
[Route("api/admin/archives")]
[ApiController]
public class ArchiveApiController(AppDbContext db, IArchiveRepository archiveRepository) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> ListCategories([FromQuery] string? search, [FromQuery] string? type, [FromQuery] int limit = 2, [FromQuery] int page = 1)
    {
        if (limit < 1) limit = 1;
        if (page < 1) page = 1;
        
        var queryTicket = db.SetEntity<Ticket>().AsNoTracking().Where(x => x.IsDeleted);
        var queryKb = db.SetEntity<KnowledgeBase>().AsNoTracking().Where(x => x.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            queryTicket = queryTicket.Where(t => t.Title.Contains(search));
            queryKb = queryKb.Where(t => t.Title.Contains(search));
        }

        var ticketQuery = queryTicket
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Description,
                x.CreatedAt,
                x.UpdatedAt,
                Type = "Ticket"
            })
            .OrderBy(t => t.CreatedAt);
        
        var kbQuery = queryKb
            .Select(x => new
            {
                x.Id,
                x.Title,
                x.Content,
                x.CreatedAt,
                x.UpdatedAt,
                Type = "Kb"
            })
            .OrderBy(t => t.CreatedAt);

        if (!string.IsNullOrEmpty(type))
        {
            if (type.Equals("ticket", StringComparison.OrdinalIgnoreCase))
            {
                var paginatedTicketsOnly = await PaginatedList<object>.CreateAsync(ticketQuery, page, limit);
                return Ok(PaginateResponse<object>.Create(paginatedTicketsOnly));
            }
        
            if (type.Equals("kb", StringComparison.OrdinalIgnoreCase))
            {
                var paginatedKBsOnly = await PaginatedList<object>.CreateAsync(kbQuery, page, limit);
                return Ok(PaginateResponse<object>.Create(paginatedKBsOnly));
            }
        }
        
        var paginatedTickets = await PaginatedList<object>.CreateAsync(ticketQuery, page, limit);
        var paginatedKBs = await PaginatedList<object>.CreateAsync(kbQuery, page, limit);

        paginatedTickets.AddRange(paginatedKBs);

        return Ok(PaginateResponse<object>.Create(paginatedTickets));
    }
    
    [HttpPost]
    public async Task<IActionResult> ActionArchive([FromBody] ArchiveActionDto dto)
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

        var response = await archiveRepository.ArchiveActionAsync(dto.Id, dto.Type, dto.Action);

        if (!response.Success) return BadRequest(response);

        return Ok(response);
    }
    
}