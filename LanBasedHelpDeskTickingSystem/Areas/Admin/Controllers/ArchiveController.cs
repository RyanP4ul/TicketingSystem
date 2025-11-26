using LanBasedHelpDeskTickingSystem.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = nameof(UserRole.Admin))]
[Route("/Admin/Archives")]
public class ArchiveController : Controller
{
    
    public IActionResult Index()
    {
        return View("Admin/Archive/Index");
    }
    
}