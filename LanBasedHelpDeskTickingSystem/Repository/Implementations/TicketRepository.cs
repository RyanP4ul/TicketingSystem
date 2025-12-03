using System.Globalization;
using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Libs;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using LanBasedHelpDeskTickingSystem.Utils;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class TicketRepository(AppDbContext db, INotifyRepository notifyRepository, IFileRepository fileRepository, IAiSupportService aiSupportService) : ITicketRepository
{
    public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
    {
        return await db.SetEntity<Ticket>()
            .Include(x => x.Category)
            .Include(x => x.Requester)
            .Include(x => x.Attachments)
            .FirstOrDefaultAsync(x => x.Id == ticketId && !x.IsDeleted);
    }

    public async Task<int> GetTotalTicketsByStatusAsync()
    {
        return await db.SetEntity<Ticket>().CountAsync(x => x.Status == "open" && !x.IsDeleted);
    }

    public async Task<int[]> GetTicketPerDay()
    {
        var today = DateTime.Today;
        var diff = (today.DayOfWeek - DayOfWeek.Monday + 7) % 7;
        var startOfWeek = today.AddDays(-1 * diff).Date;
        var endOfWeek = startOfWeek.AddDays(6).Date;
        var allTickets = await db.SetEntity<Ticket>().ToListAsync();

        var ticketsByDay = allTickets
            .Where(t => t.CreatedAt.Date >= startOfWeek && t.CreatedAt.Date <= endOfWeek)
            .GroupBy(t => t.CreatedAt.Date)
            .ToDictionary(g => g.Key, g_ => g_.Count());

        var dailyCounts = new int[7];

        for (var i = 0; i < 7; i++)
        {
            var currentDay = startOfWeek.AddDays(i);
            dailyCounts[i] = ticketsByDay.GetValueOrDefault(currentDay, 0);
        }

        return dailyCounts;
    }
    
    public async Task<Dictionary<string, int>> GetTicketCategoryPopularityAsync()
    {
        var now = DateTime.Now;
        var currentDayOfWeek = (int)now.DayOfWeek; 
        var daysToSubtract = currentDayOfWeek == 0 ? 6 : currentDayOfWeek - 1;
        var startOfWeek = now.Date.AddDays(-daysToSubtract);
        var endOfWeek = startOfWeek.AddDays(7);

        var dbCounts = await db.SetEntity<Ticket>()
            .Where(t => t.CreatedAt >= startOfWeek 
                        && t.CreatedAt < endOfWeek 
                        && !t.IsDeleted)
            .Include(x => x.Category)
            .GroupBy(t => new { t.Category!.Name })
            .Select(g => new 
            { 
                g.Key.Name,
                Count = g.Count() 
            })
            .ToListAsync();

        var results = new Dictionary<string, int>();

        foreach (var item in dbCounts)
        {
            results[item.Name] = item.Count;
        }

        return results;
    }

    public async Task<IEnumerable<Ticket>> GetRecentTicketsAsync(int limit)
    {
        return await db.SetEntity<Ticket>()
            .Include(t => t.Assigned)
            .Include(t => t.Requester)
            .Include(c => c.Category)
            .OrderByDescending(t => t.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<ApiResultResponse> CreateTicketAsync(int userId, string title, string description, int categoryId, string room, List<IFormFile> files)
    {
        if (files.Count > 5)
        {
            return ApiResultResponse.Error("You can upload a maximum of 5 files");
        }
        
        var isRelevant = await aiSupportService.IsTicketRelevantAsync(title, description);
        
        if (!isRelevant)
        {
            return ApiResultResponse.Error("The ticket content is not relevant!");
        }

        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var priority = await aiSupportService.AnalyzeTicketPriorityAsync(title, description);
            
            var ticket = new Ticket
            {
                TicketNumber = TicketNumberGenerator.GenerateTicketNumber(),
                Title = title,
                Description = description,
                CategoryId = categoryId,
                Room = room,
                RequesterId = userId,
                Status = "open",
                Priority = priority.ToLower(),
                CreatedAt = DateTime.UtcNow
            };

            await db.SetEntity<Ticket>().AddAsync(ticket);
            await db.SaveChangesAsync();

            var fileResponse = await fileRepository.SaveTicketAttachmentsAsync(ticket.Id, files);

            if (!fileResponse.Success)
            {
                await transaction.RollbackAsync();
                return ApiResultResponse.Error("Failed to create ticket attachments");
            }

            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Ticket created successfully");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("Failed to create ticket");
        }
    }

    public async Task<ApiResultResponse> UpdateTicketByAdminAsync(int userId, int ticketId, int assigned, string status, string priority, string note)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null)
            {
                return ApiResultResponse.Error("Ticket not found");
            }

            if (status == "resolved" || status == "closed")
            {
                return ApiResultResponse.Error("Cannot update a resolved or closed ticket");
            }

            var user = await db.SetEntity<User>().FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                return ApiResultResponse.Error("User not found");
            }

            if (ticket.AssignedId != user.Id)
            {
                return ApiResultResponse.Error("You are not assigned to this ticket");
            }

            if (!string.IsNullOrEmpty(note)) ticket.Resolution = note;

            ticket.Status = status;
            ticket.Priority = priority;
            ticket.AssignedId = assigned;
            ticket.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            
            if (status == "resolved") await notifyRepository.CreateNotification(ticket.Id, ticket.RequesterId, "ticket_resolved");
            if (status == "closed") await notifyRepository.CreateNotification(ticket.Id, ticket.RequesterId, "ticket_rejected");
            
            if (status == "resolved" || status == "closed")
            {
                ticket.ResolvedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Ticket updated successfully");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("Failed to update ticket");
        }
    }

    public async Task<ApiResultResponse> UpdateTicketByUserAsync(int ticketId, string title, string description, int categoryId, string room, List<IFormFile> files)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null) return ApiResultResponse.Error("Ticket not found");

            ticket.Title = title;
            ticket.Description = description;
            ticket.CategoryId = categoryId;
            ticket.Room = room;

            await db.SaveChangesAsync();
            
            var fileResponse = await fileRepository.SaveTicketAttachmentsAsync(ticket.Id, files);
            
            if (!fileResponse.Success)
            {
                await transaction.RollbackAsync();
                return ApiResultResponse.Error("Failed to create ticket attachments");
            }
            
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Ticket updated successfully");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("Failed to update ticket");
        }
    }

    public async Task<ApiResultResponse> DeleteTicketAsync(int userId, int ticketId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null) return ApiResultResponse.Error("Ticket not found");

            ticket.IsDeleted = true;
            ticket.DeletedBy = userId;
            ticket.DeletedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Ticket deleted successfully");
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("Failed to delete ticket");
        }
    }

    public async Task<ApiResultResponse> DeleteTicketAttachmentByUserAsync(int ticketId, int attachmentId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var attachment = await db.SetEntity<Attachment>()
                .FirstOrDefaultAsync(x => x.TicketId == ticketId && x.id == attachmentId);

            if (attachment == null)
            {
                return ApiResultResponse.Error("Attachment not found");
            }

            db.SetEntity<Attachment>().Remove(attachment);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResultResponse.Ok("Attachment deleted successfully");
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("Failed to delete ticket attachment");
        }
    }
}