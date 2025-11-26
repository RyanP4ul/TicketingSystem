using LanBasedHelpDeskTickingSystem.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("/Admin/Users")]
public class UserController : Controller
{
   
    public IActionResult Index()
    {
        return View("Admin/User/Index");
    }
    
}