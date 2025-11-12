using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IUserRepository
{
    public Task<Ticket?> GetTicketByIdAsync(int userId, int ticketId);
    public Task<IEnumerable<Ticket>> GetRecentTicketsByUserIdAsync(int userId, int count);
}