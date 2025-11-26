using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Libs;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface ITicketRepository
{
    public Task<Ticket?> GetTicketByIdAsync(int ticketId);
    public Task<int> GetTotalTicketsByStatusAsync();
    public Task<IEnumerable<Ticket>> GetRecentTicketsAsync(int limit);
    public Task<int[]> GetTicketPerDay();
    public Task<Dictionary<string, int>> GetTicketCategoryPopularityAsync();
    public Task<ApiResultResponse> CreateTicketAsync(int requesterId, string title, string description, int categoryId, string priority, List<IFormFile> files);
    public Task<ApiResultResponse> UpdateTicketByAdminAsync(int userId, int ticketId, int assigned, string status, string note);
    public Task<ApiResultResponse> UpdateTicketByUserAsync(int ticketId, string title, string description, int categoryId, string priority, List<IFormFile> files);
    public Task<ApiResultResponse> DeleteTicketAsync(int userId, int ticketId);
    public Task<ApiResultResponse> DeleteTicketAttachmentByUserAsync(int ticketId, int attachmentId);
}