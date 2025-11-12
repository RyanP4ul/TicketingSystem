namespace LanBasedHelpDeskTickingSystem.Repository.Interfaces;

public interface IAdminDashboardRepository
{
    public Task<int> GetTotalTickets();
    public Task<int> GetPendingTickets();
    public Task<int> GetInProgressTickets();
    public Task<int> GetResolvedTickets();
    public Task<int> GetClosedTickets();
}