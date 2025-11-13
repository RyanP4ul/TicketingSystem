using LanBasedHelpDeskTickingSystem.Entities.Enums;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.User.Controllers;

[Area("User")]
[Route("User/Tickets")]
[Authorize(Roles = nameof(UserRole.User))]
public class TicketController (IUserRepository userRepository, ICategoryRepository categoryRepository) : Controller
{
    
    [HttpGet]
    public async Task <IActionResult> Index()
    {
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        var tickets = await userRepository.GetRecentTicketsByUserIdAsync(userId, 5);
        ViewBag.Tickets = tickets;
        return View("Student/Ticket/Index");
    }

    [HttpGet("New")]
    public async Task<IActionResult> NewTicket()
    {
        var categories = await categoryRepository.GetAllCategoriesAsync();
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        
        return View("Student/Ticket/NewTicket", new NewTicketViewModel
        {
            UserId = userId,
            Categories = categories
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ViewTicket(int id)
    {
        var userId = Convert.ToInt32(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "");
        var ticket =  await userRepository.GetTicketByIdAsync(userId, id);

        if (ticket == null)
        {
            return RedirectToAction("Index");
        }
        
        var categories = await categoryRepository.GetAllCategoriesAsync();

        return View("Student/Ticket/ViewTicket", new UserDetailViewModel
        {
            Ticket = ticket,
            Categories = categories
        });
    }

}