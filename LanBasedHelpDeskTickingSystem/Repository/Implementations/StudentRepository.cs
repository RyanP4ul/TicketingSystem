using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class StudentRepository(AppDbContext db) : IStudentRepository
{

    public async Task<int> GetTicketCountAsync(int userId, string? status = null)
    {
        var query = db.SetEntity<Ticket>().Where(x => x.RequesterId == userId);

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(x => x.Status == status);
        }

        return await query.CountAsync();
    }
    
    public async Task<IEnumerable<Ticket>> GetRecentTicketsEnumerableAsync(int userId)
    {
        return await db.SetEntity<Ticket>().Include(x => x.Requester).Include(x => x.Category)
            .Where(t => t.RequesterId == userId && (t.Status == "open" || t.Status == "in_progress"))
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .ToListAsync();
    }
    
}