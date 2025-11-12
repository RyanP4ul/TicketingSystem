using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class UserRepository(AppDbContext db) : IUserRepository
{
    
    public async Task<Ticket?> GetTicketByIdAsync(int userId, int ticketId)
    {
        return await db.SetEntity<Ticket>()
            .Include(x => x.Category)
            .FirstOrDefaultAsync(t => t.Id == ticketId && t.RequesterId == userId && (t.Status == "open" || t.Status == "in_progress"));
    }
    
    public async Task<IEnumerable<Ticket>> GetRecentTicketsByUserIdAsync(int userId, int count)
    {
        return await db.SetEntity<Ticket>()
            .Where(t => t.RequesterId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
    
}