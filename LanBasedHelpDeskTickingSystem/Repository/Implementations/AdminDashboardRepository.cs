using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Repository.Implementations;

public class AdminDashboardRepository(AppDbContext db) : IAdminDashboardRepository
{

    public async Task<int> GetTotalTickets()
    {
        return await db.SetEntity<Ticket>().CountAsync();
    }
    
    public async Task<int> GetPendingTickets()
    {
        return await db.SetEntity<Ticket>().CountAsync(x => x.Status == "pending");
    }
    
    public async Task<int> GetInProgressTickets()
    {
        return await db.SetEntity<Ticket>().CountAsync(x => x.Status == "in_progress");
    }
    
    public async Task<int> GetResolvedTickets()
    {
        return await db.SetEntity<Ticket>().CountAsync(x => x.Status == "resolved");
    }
    
    public async Task<int> GetClosedTickets()
    {
        return await db.SetEntity<Ticket>().CountAsync(x => x.Status == "closed");
    }
    
}