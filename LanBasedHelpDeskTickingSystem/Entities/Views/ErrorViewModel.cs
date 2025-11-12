namespace LanBasedHelpDeskTickingSystem.Entities.Views;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}