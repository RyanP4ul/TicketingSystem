using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Libs;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api.Admin;

[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/admin/tickets")]
[ApiController]
public class TicketApiController(AppDbContext db, ITicketRepository ticketRepository) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> ListTickets([FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? category, [FromQuery] string? priority, [FromQuery] int limit = 2, [FromQuery] int page = 1)
    {
        if (limit < 1) limit = 1;
        if (page < 1) page = 1;
        
        var query = db.SetEntity<Ticket>()
            .Include(x => x.Category)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Title.Contains(search));
        if (!string.IsNullOrEmpty(status)) query = query.Where(t => t.Status == status);
        if (!string.IsNullOrEmpty(category)) query = query.Where(t => t.Category != null && t.Category.Name == category);
        if (!string.IsNullOrEmpty(priority)) query = query.Where(t => t.Priority == priority);

        var ticketsQuery = query.Include(x => x.Requester).Where(x => !x.IsDeleted).OrderBy(t => t.Id);
        var paginatedTickets = await PaginatedList<Ticket>.CreateAsync(ticketsQuery, page, limit);
        
        return Ok(PaginateResponse<Ticket>.Create(paginatedTickets));
    }
    
    [HttpGet("total")]
    public async Task<IActionResult> GetTotalTicketsByStatus()
    {
        var totalTickets = await ticketRepository.GetTotalTicketsByStatusAsync();
        return Ok(ApiResponse<int>.Ok(totalTickets));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateTicket([FromBody] AdminTicketUpdateViewModel model)
    {
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        var response = await ticketRepository.UpdateTicketByAdminAsync(userId, model.ticketId, model.assigned, model.status, model.priority, model.notes);
        
        if (!response.Success) return BadRequest(response);
        
        return Ok(response);
    }
    
}