namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class AdminTicketUpdateViewModel
{
    public int ticketId { get; set; }
    public string status { get; set; }
    public string assigned { get; set; }
    public string notes { get; set; }
}