using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Libs;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api.User;

[Authorize(Roles = nameof(UserRole.User))]
[Route("api/user/tickets")]
[ApiController]
public class TicketApiController(AppDbContext db, ITicketRepository ticketRepository) : ControllerBase
{
    
    [HttpGet]
    [EnableRateLimiting("default")]
    public async Task<IActionResult> ListTickets([FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? category, [FromQuery] string? priority, [FromQuery] int limit = 2, [FromQuery] int page = 1)
    {
        if (limit < 1) limit = 1;
        if (page < 1) page = 1;
        
        var query = db.SetEntity<Ticket>()
            .Include(x => x.Attachments)
            .Include(x => x.Category)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.Title.Contains(search));
        if (!string.IsNullOrEmpty(status)) query = query.Where(t => t.Status == status);
        if (!string.IsNullOrEmpty(category)) query = query.Where(t => t.Category != null && t.Category.Name == category);
        if (!string.IsNullOrEmpty(priority)) query = query.Where(t => t.Priority == priority);

        var ticketsQuery = query.Include(x => x.Requester).Include(x => x.Attachments).Select(x => new
        {
            x.Id,
            x.Title,
            x.Status,
            x.Priority,
            x.Description,
            x.CreatedAt,
            Category = new
            {
                x.Category!.Id,
                x.Category!.Name
            },
            Requester = new
            {
                x.Requester!.Username,
            },
            Attachments = x.Attachments.Select(a => new
            {
                a.FileName,
                a.FileExtension
            }).ToList(),
        }).OrderBy(t => t.Id);
        var paginatedTickets = await PaginatedList<object>.CreateAsync(ticketsQuery, page, limit);
        
        return Ok(PaginateResponse<object>.Create(paginatedTickets));
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromForm] CreateTicketDto newTicket)
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
        
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        var response = await ticketRepository.CreateTicketAsync(userId, newTicket.Title, newTicket.Description, newTicket.CategoryId, newTicket.Priority, newTicket.Files);
        
        if (!response.Success) return BadRequest(response);

        return Ok(response);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTicket([FromRoute] int id, [FromForm] ViewTicketDto updatedTicket)
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
            var response = await ticketRepository.UpdateTicketByUserAsync(id, updatedTicket.Title, updatedTicket.Description, updatedTicket.CategoryId, updatedTicket.Priority, updatedTicket.Files);
            
            if (!response.Success) return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest(ApiResultResponse.Error("Failed to update ticket"));
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket([FromRoute] int id)
    {
        try
        {
            var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
            var response = await ticketRepository.DeleteTicketAsync(userId, id);

            if (!response.Success) return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest(ApiResultResponse.Error("Failed to delete ticket"));
        }
    }
    
    [HttpPost("attachment")]
    public async Task<IActionResult> DeleteTicketAttachment([FromBody] RemoveAttachmentDto dto)
    {
        try
        {
            var response = await ticketRepository.DeleteTicketAttachmentByUserAsync(dto.TicketId, dto.AttachmentId);

            if (!response.Success) return BadRequest(response);
            
            return Ok(response);
        }
        catch (Exception)
        {
            return BadRequest(ApiResultResponse.Error("Failed to delete ticket attachment"));
        }
    }
    
}