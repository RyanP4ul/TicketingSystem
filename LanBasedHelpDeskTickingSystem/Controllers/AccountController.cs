using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Services;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers;

public class AccountController(IUserService userService, IJwtService jwtService) : Controller
{
    
    [Authorize] [HttpGet] public IActionResult Index() => View();
    
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Index", "Home");
    }
    
}