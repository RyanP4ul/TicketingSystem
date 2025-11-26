using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class UserViewModel
{
    public required IEnumerable<Ticket> Tickets { get; set; }
    public required int TotalTickets { get; set; }
    public required int PendingTickets { get; set; }
    public required int InProgressTickets { get; set; }
    public required int ResolvedTickets { get; set; }
    public required int ClosedTickets { get; set; }
}