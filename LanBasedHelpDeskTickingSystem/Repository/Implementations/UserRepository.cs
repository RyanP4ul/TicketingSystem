using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class UserRepository(AppDbContext db) : IUserRepository
{
    
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await db.SetEntity<User>().FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<IEnumerable<User>> GetUserByIdAsync()
    {
        return await db.SetEntity<User>().ToListAsync();
    }
    
    public async Task<Ticket?> GetTicketByIdAsync(int userId, int ticketId)
    {
        return await db.SetEntity<Ticket>()
            .Include(x => x.Category)
            .Include(x => x.Attachments)
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
    
    public async Task<IEnumerable<User>> GetAllTechniciansAsync()
    {
        return await db.SetEntity<User>()
            .Where(u => u.Roles == Entities.Enums.UserRole.Admin)
            .ToListAsync();
    }

    public async Task<User?> CreateUserByGoogleAsync(string name, string email)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();
        
        try
        {
            var user = new User
            {
                Username = name,
                Email = email,
                IsGoogleOauth = true
            };
            
            db.SetEntity<User>().Add(user);
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await transaction.RollbackAsync();
        }

        return null;
    }
    
}