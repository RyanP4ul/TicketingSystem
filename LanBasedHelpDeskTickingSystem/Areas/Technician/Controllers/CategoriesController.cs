using LanBasedHelpDeskTickingSystem.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Technician.Controllers;

[Area("Technician")]
[Authorize(Roles = nameof(UserRole.Technician))]
[Route("/Technician/Categories")]
public class CategoriesController : Controller
{

    public IActionResult Index()
    {
        return View("Technician/Categories/Index");
    }
}