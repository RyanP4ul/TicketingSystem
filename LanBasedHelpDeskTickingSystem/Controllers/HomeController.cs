using System.Diagnostics;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == false) return RedirectToAction("Login", "Auth");
        return User.IsInRole("Admin") ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "User");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}