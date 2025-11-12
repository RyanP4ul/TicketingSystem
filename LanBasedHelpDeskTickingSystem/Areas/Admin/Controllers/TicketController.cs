using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("/Admin/Tickets")]
public class TicketController(ITicketRepository ticketRepository) : Controller
{
    
    [HttpGet]
    public IActionResult Index()
    {
        return View("Admin/Ticket/TicketLists");
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Ticket(int id)
    {
        var ticket = await ticketRepository.GetTicketByIdAsync(id);

        if (ticket == null)
        {
            return RedirectToAction("Index");
        }
        
        return View("Admin/Ticket/ViewTicket", ticket);
    }
    
}