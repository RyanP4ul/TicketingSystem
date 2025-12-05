using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
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
            .Where(u => u.Roles == Entities.Enums.UserRole.Technician || u.Roles == Entities.Enums.UserRole.Admin)
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

    public async Task<ApiResultResponse> UpdateUser(int userId, string email, string username, string role)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var user = await db.SetEntity<User>().FirstOrDefaultAsync(x => x.Id == userId);
            
            if (user == null) return ApiResultResponse.Error("User not found.");
            
            user.Email = email;
            user.Username = username;
            user.Roles = Enum.Parse<Entities.Enums.UserRole>(role);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return ApiResultResponse.Ok("User updated successfully.");
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while updating the user.");
        }
    }
    
}