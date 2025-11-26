using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class UserKbViewModel
{
    public required IEnumerable<Category> Categories { get; set; }
}