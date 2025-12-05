using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController(IAdminDashboardRepository adminDashboardRepository, ICategoryRepository categoryRepository, ITicketRepository ticketRepository) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await categoryRepository.GetAllCategoriesAsync();
        var recentTickets = await ticketRepository.GetRecentTicketsAsync(5);
        var perDay = await ticketRepository.GetTicketPerDay();
        var popularCategories = await ticketRepository.GetTicketCategoryPopularityAsync();
        
        return View("Admin/Index", new AdminViewModel
        {
            Categories = categories,
            Tickets = recentTickets,
            PopularCategories = popularCategories,
            TicketsPerDay = perDay,
            TotalTickets = await adminDashboardRepository.GetTotalTickets(),
            PendingTickets = await adminDashboardRepository.GetStatusTickets("pending"),
            InProgressTickets = await adminDashboardRepository.GetStatusTickets("in_progress"),
            ResolvedTickets = await adminDashboardRepository.GetStatusTickets("resolved"),
            ClosedTickets = await adminDashboardRepository.GetStatusTickets("closed")
        });
    }
    
}