namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class AdminTicketUpdateViewModel
{
    public int ticketId { get; set; }
    public string status { get; set; }
    public int assigned { get; set; }
    public string priority { get; set; }
    public string notes { get; set; }
}