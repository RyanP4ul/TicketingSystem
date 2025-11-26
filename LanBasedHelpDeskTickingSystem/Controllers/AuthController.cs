using System.Security.Claims;
using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers;

public class AuthController(AppDbContext db, IUserRepository _userRepository, IJwtService _jwtService) : Controller
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