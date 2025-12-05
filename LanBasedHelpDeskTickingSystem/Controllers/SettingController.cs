using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers;

[Authorize]
public class SettingController : Controller
{
    
    [HttpGet]
    public IActionResult Index()
    {
        return View("Setting/Index");
    }
    
}