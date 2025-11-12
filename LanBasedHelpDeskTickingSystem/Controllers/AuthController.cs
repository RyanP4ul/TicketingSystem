using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers;

public class AuthController : Controller
{

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Account");
        }
        
        return View();
    }
    
    [HttpGet] public IActionResult Register() => View();
    
}