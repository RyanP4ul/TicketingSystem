using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class CommentRepository(AppDbContext db, INotifyRepository notifyRepository) : ICommentRepository
{
    
    public async Task<Comment?> GetCommentByIdAsync(int commentId)
    {
        return await db.SetEntity<Comment>()
            .Include(x => x.User)
            .Include(x => x.Ticket)
            .Select(x => new Comment
            {
                Id = x.Id,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                User = new User
                {
                    Id = x.User!.Id,
                    Username = x.User!.Username,
                    Roles = x.User.Roles
                }
            })
            .FirstOrDefaultAsync(c => c.Id == commentId);
    }
    
    public async Task<IEnumerable<Comment>> GetCommentsByTicketIdAsync(int ticketId)
    {
        return await db.SetEntity<Comment>()
            .Include(x => x.User)
            .Include(x => x.Ticket)
            .Select(x => new Comment
            {
                Id = x.Id,
                TicketId = x.TicketId,
                Content = x.Content,
                CreatedAt = x.CreatedAt,
                User = new User
                {
                    Id = x.User!.Id,
                    Username = x.User!.Username,
                    Roles = x.User.Roles
                }
            })
            .Where(c => c.TicketId == ticketId)
            .ToListAsync();
    }
    
    public async Task<ApiResultResponse> CreateCommentAsync(int ticketId, int userId, string content)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var comment = new Comment
            {
                TicketId = ticketId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await db.AddAsync(comment);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            
            await notifyRepository.CreateNotification(comment.TicketId,  userId, "comment");
            
            var createdComment = await GetCommentByIdAsync(comment.Id);
            
            return ApiResultResponse.Ok(System.Text.Json.JsonSerializer.Serialize(createdComment));
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return ApiResultResponse.Error("An error occurred while creating the comment.");
        }
    }
    
}