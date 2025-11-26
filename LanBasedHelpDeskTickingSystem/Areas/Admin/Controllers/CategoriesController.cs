using LanBasedHelpDeskTickingSystem.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("/Admin/Categories")]
public class CategoriesController : Controller
{

    public IActionResult Index()
    {
        return View("Admin/Categories/Index");
    }
    
}