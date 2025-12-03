using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface ICommentRepository
{
    public Task<Comment?> GetCommentByIdAsync(int commentId);
    public Task<IEnumerable<Comment>> GetCommentsByTicketIdAsync(int ticketId);
    public Task<ApiResultResponse> CreateCommentAsync(int ticketId, int userId, string content);
}