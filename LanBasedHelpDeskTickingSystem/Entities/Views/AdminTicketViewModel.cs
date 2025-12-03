using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class AdminTicketViewModel
{
    public required Ticket Ticket { get; set; }
    public required IEnumerable<User> Users { get; set; }
    public required IEnumerable<Comment> Comments { get; set; }
}