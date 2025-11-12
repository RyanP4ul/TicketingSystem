using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Libs;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class TicketRepository(AppDbContext db) : ITicketRepository
{

    public async Task<Ticket?> GetTicketByIdAsync(int ticketId)
    {
        return await db.SetEntity<Ticket>().Include(x => x.Category).Include(x => x.Requester).FirstOrDefaultAsync(x => x.Id == ticketId);
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

    public async Task UpdateTicketAdminNoteAsync(int ticketId, string status, string note)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        
        try
        {
            var ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null)
            {
                throw new Exception("Ticket not found");
            }

            if (!string.IsNullOrEmpty(note)) ticket.Resolution = note;

            ticket.Status = status;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
        }
    }

    public async Task UpdateTicketAsync(int ticketId, string description, int categoryId, string priority)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null) return;

            ticket.Description = description;
            ticket.CategoryId = categoryId;
            ticket.Priority = priority;
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
        }
    }

    public async Task DeleteTicketAsync(int ticketId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var ticket = await GetTicketByIdAsync(ticketId);

            if (ticket == null) return;
            
            db.SetEntity<Ticket>().Remove(ticket);
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
        }

        
    }
    
}