using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class NewTicketViewModel
{
    public int UserId { get; set; }
    public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();
}