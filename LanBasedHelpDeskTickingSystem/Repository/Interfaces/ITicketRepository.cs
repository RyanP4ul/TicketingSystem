using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Libs;

namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface ITicketRepository
{
    // public Task<IEnumerable<Ticket>> GetTickets();
    // public Task<IEnumerable<Ticket>> GetTickets(int limit);
    // public Task<PaginatedList<Ticket>> GetPaginatedTickets(int page, int limit);
    public Task<Ticket?> GetTicketByIdAsync(int ticketId);
    public Task<IEnumerable<Ticket>> GetRecentTicketsAsync(int limit);
    public Task UpdateTicketAdminNoteAsync(int ticketId, string status, string note);
    public Task UpdateTicketAsync(int ticketId, string description, int categoryId, string priority);
    public Task DeleteTicketAsync(int ticketId);
}