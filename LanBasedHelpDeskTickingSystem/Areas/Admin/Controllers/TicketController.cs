using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("/Admin/Tickets")]
public class TicketController(ICategoryRepository categoryRepository, ICommentRepository commentRepository, ITicketRepository ticketRepository, IUserRepository userRepository) : Controller
{
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await categoryRepository.GetAllCategoriesAsync();
        return View("Admin/Ticket/TicketLists", categories);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Ticket(int id)
    {
        var ticket = await ticketRepository.GetTicketByIdAsync(id);

        if (ticket == null) return RedirectToAction("Index");
        
        var users = await userRepository.GetAllTechniciansAsync();
        var comments = await commentRepository.GetCommentsByTicketIdAsync(id);
        
        return View("Admin/Ticket/ViewTicket", new AdminTicketViewModel
        {
            Ticket = ticket,
            Users = users,
            Comments = comments
        });
    }
    
}