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

namespace LanBasedHelpDeskTickingSystem.Controllers.Api.User;

[Authorize(Roles = nameof(UserRole.User))]
[Route("api/user/tickets")]
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

        var ticketsQuery = query.Include(x => x.Requester).OrderBy(t => t.Id);
        var paginatedTickets = await PaginatedList<Ticket>.CreateAsync(ticketsQuery, page, limit);
        
        return Ok(PaginateResponse<Ticket>.Create(paginatedTickets));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto newTicket)
    {
        try
        {
            await ticketRepository.CreateTicketAsync(newTicket.RequesterId, newTicket.Title, newTicket.Description, newTicket.CategoryId, newTicket.Priority);
            return Ok(ApiResponse<string>.Ok("Ticket created successfully."));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse<string>.Error("Failed to create ticket"));
        }
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket([FromRoute] int id, [FromBody] ViewTicketDto updatedTicket)
    {
        try
        {
            await ticketRepository.UpdateTicketAsync(id, updatedTicket.Description, updatedTicket.CategoryId, updatedTicket.Priority);
            return Ok(ApiResponse<string>.Ok("Ticket updated successfully."));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse<string>.Error("Failed to update ticket"));
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket([FromRoute] int id)
    {
        try
        {
            await ticketRepository.DeleteTicketAsync(id);

            return Ok(ApiResponse<string>.Ok("Ticket deleted successfully."));
        }
        catch (Exception)
        {
            return BadRequest(ApiResponse<string>.Error("Failed to delete ticket"));
        }
    }
    
}