using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IStudentRepository
{
    public Task<int> GetTicketCountAsync(int userId, string? status = null);
    public Task<IEnumerable<Ticket>> GetRecentTicketsEnumerableAsync(int userId);
}