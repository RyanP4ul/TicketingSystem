using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController(IAdminDashboardRepository adminDashboardRepository, ICategoryRepository categoryRepository, ITicketRepository ticketRepository) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await categoryRepository.GetAllCategoriesAsync();
        var recentTickets = await ticketRepository.GetRecentTicketsAsync(10);
        
        return View(new AdminViewModel
        {
            Categories = categories,
            Tickets = recentTickets,
            TotalTickets = await adminDashboardRepository.GetTotalTickets(),
            PendingTickets = await adminDashboardRepository.GetPendingTickets(),
            InProgressTickets = await adminDashboardRepository.GetInProgressTickets(),
            ResolvedTickets = await adminDashboardRepository.GetResolvedTickets(),
            ClosedTickets = await adminDashboardRepository.GetClosedTickets()
        });
    }
    
}