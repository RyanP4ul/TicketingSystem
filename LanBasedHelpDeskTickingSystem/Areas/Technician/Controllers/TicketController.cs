using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Technician.Controllers;

[Area("Technician")]
[Authorize(Roles = nameof(UserRole.Technician))]
[Route("/Technician/Tickets")]
public class TicketController(ICategoryRepository categoryRepository, ICommentRepository commentRepository, ITicketRepository ticketRepository, IUserRepository userRepository) : Controller
{
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var categories = await categoryRepository.GetAllCategoriesAsync();
        return View("Technician/Ticket/TicketLists", categories);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Ticket(int id)
    {
        var ticket = await ticketRepository.GetTicketByIdAsync(id);

        if (ticket == null) return RedirectToAction("Index");
        
        var users = await userRepository.GetAllTechniciansAsync();
        var comments = await commentRepository.GetCommentsByTicketIdAsync(id);
        
        return View("Technician/Ticket/ViewTicket", new AdminTicketViewModel
        {
            Ticket = ticket,
            Users = users,
            Comments = comments
        });
    }
    
}