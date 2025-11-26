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
        var categories = await categoryRepository.GetAllCategoriesAsync();
        return View("Student/Ticket/Index", categories);
    }

    [HttpGet("New")]
    public async Task<IActionResult> NewTicket()
    {
        ViewBag.Categories = await categoryRepository.GetAllCategoriesAsync();
        
        return View("Student/Ticket/NewTicket");
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

        ViewBag.Ticket = ticket;
        ViewBag.Categories = categories;

        return View("Student/Ticket/ViewTicket");
    }

}