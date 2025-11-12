using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class UserDetailViewModel
{
    public required Ticket Ticket;
    public required IEnumerable<Category> Categories;
}