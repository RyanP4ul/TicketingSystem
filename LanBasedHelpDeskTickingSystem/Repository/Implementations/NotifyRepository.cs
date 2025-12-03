using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class NotifyRepository(AppDbContext db) : INotifyRepository
{
    
    public async Task<Notification?> GetNotificationById(int id)
    {
        return await db.SetEntity<Notification>().Include(x => x.Ticket).Include(x => x.User).FirstOrDefaultAsync(n => n.Id == id);
    }
    
    public async Task<IEnumerable<dynamic>> GetLatestNotifiesById(int userId)
    {
        var notifies = await db.SetEntity<Notification>()
            .Include(x => x.Ticket)
            .Include(x => x.User)
            .OrderByDescending(n => n.CreatedAt)
            .Where(x => x.UserId == userId)
            .Select(x => new
            {
                id = x.Id,
                message = x.message,
                is_read = x.IsRead,
                created_at = x.CreatedAt,
                Ticket = new
                {
                    id = x.Ticket!.Id,
                    title = x.Ticket!.Title
                },
                User = new
                {
                    id = x.User!.Id,
                    name = x.User!.Username,
                    role = x.User!.Roles.ToString()
                }
            })
            .Take(5)
            .ToListAsync();

        return notifies;
    }
    
    public async Task UpdateReadStatus(int notificationId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var notification = await db.SetEntity<Notification>().FirstOrDefaultAsync(n => n.Id == notificationId && !n.IsRead);

            if (notification == null) return;
            
            notification.IsRead = true;
            
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
        }
    }
    
    public async Task CreateNotification(int ticketId, int userId, string type = "comment")
    {
        var ticket = await db.SetEntity<Ticket>().Include(x => x.Requester).FirstOrDefaultAsync(x => x.Id == ticketId);
        
        if (ticket == null || ticket.Requester == null || ticket.Requester.Id == userId) return;

        var message = "";

        if (type == "comment")
        {
            message = $"{ticket.Requester.Username} commented on ticket '{ticket.Title}'";
        }
        else if (type == "status_update")
        {
            message = $"The status of ticket '{ticket.Title}' has been updated";
        }
        else if (type == "ticket_resolved")
        {
            message = $"Your ticket '{ticket.Title}' has been resolved";
        }
        else if (type == "ticket_rejected")
        {
            message = $"Your ticket '{ticket.Title}' has been rejected";
        }
        else
        {
            message = $"There is an update on your ticket '{ticket.Title}'";
        }
        
        await db.SetEntity<Notification>().AddAsync(new Notification
        {
            TicketId = ticketId,
            UserId = ticket.Requester.Id,
            message = message
        });
            
        await db.SaveChangesAsync();
    }
    
    public async Task ClearNotificationsByUserId(int userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var notifications = await db.SetEntity<Notification>()
                .Where(n => n.UserId == userId).ToListAsync();

            db.SetEntity<Notification>().RemoveRange(notifications);
            await db.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            await transaction.RollbackAsync();
        }
    }
    
}