using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.User.Controllers;

[Route("User")]
[Authorize(Roles = nameof(UserRole.User))]
public class UserController(IStudentRepository studentRepository) : Controller
{
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        var tickets = await studentRepository.GetRecentTicketsEnumerableAsync(userId);
        
        return View("Student/Index", new UserViewModel
        {
            Tickets = tickets,
            TotalTickets = await studentRepository.GetTicketCountAsync(userId),
            PendingTickets = await studentRepository.GetTicketCountAsync(userId, "pending"),
            InProgressTickets = await studentRepository.GetTicketCountAsync(userId, "in_progress"),
            ResolvedTickets = await studentRepository.GetTicketCountAsync(userId, "resolved"),
            ClosedTickets = await studentRepository.GetTicketCountAsync(userId, "closed")
        });
    }
    
}