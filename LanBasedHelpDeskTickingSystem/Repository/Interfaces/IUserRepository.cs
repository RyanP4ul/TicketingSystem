using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<IEnumerable<User>> GetUserByIdAsync();
    public Task<Ticket?> GetTicketByIdAsync(int userId, int ticketId);
    public Task<IEnumerable<Ticket>> GetRecentTicketsByUserIdAsync(int userId, int count);
    public Task<IEnumerable<User>> GetAllTechniciansAsync();
    public Task<User?> CreateUserByGoogleAsync(string name, string email);
    public Task<ApiResultResponse> UpdateUser(int userId, string email, string username, string role);
}