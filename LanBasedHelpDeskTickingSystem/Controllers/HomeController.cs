using System.Diagnostics;
using LanBasedHelpDeskTickingSystem.Entities.Views;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
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
    
    // [HttpGet("login/google")]
    // public IActionResult GoogleLogin()
    // {
    //     // var redirectUrl = Url.Action(nameof(GoogleResponse));
    //     var redirectUrl = Url.Action(nameof(GoogleResponse), "AuthApi", null, Request.Scheme);
    //     var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
    //     Console.WriteLine("Initiating Google login");
    //     Console.WriteLine($"Redirect URL: {redirectUrl}");
    //     return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    // }
    //
    // [HttpGet("signin-google")]
    // public async Task<IActionResult> GoogleResponse()
    // {
    //     var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
    //
    //     if (!result.Succeeded || result.Principal == null)
    //     {
    //         Console.WriteLine("External login failed");
    //         return BadRequest("External login failed.");
    //     }
    //
    //     // var email = result.Principal.FindFirstValue(ClaimTypes.Email);
    //     // var name = result.Principal.FindFirstValue(ClaimTypes.Name);
    //     //
    //     // if (email == null)
    //     // {
    //     //     Console.WriteLine("Email claim not found");
    //     //     return BadRequest("Email claim not found.");
    //     // }
    //     //
    //     // if (name == null)
    //     // {
    //     //     Console.WriteLine("Name claim not found");
    //     //     return BadRequest("Name claim not found.");
    //     // }
    //     //
    //     // // Check if user exists
    //     // Console.WriteLine("Check User Exists");
    //     // var user = await userRepository.GetUserByEmailAsync(email);
    //     // if (user == null)
    //     // {
    //     //     Console.WriteLine("No Account");
    //     //     user = await userRepository.CreateUserByGoogleAsync(name, email);   
    //     //     if (user == null) return BadRequest("User creation failed.");
    //     // }
    //     //
    //     // Console.WriteLine("Generate Token");
    //     //
    //     // var token = jwtService.GenerateToken(user);
    //     //
    //     // Response.Cookies.Append("jwt", token, new CookieOptions
    //     // {
    //     //     HttpOnly = true,
    //     //     Secure = false, // true in production
    //     //     SameSite = SameSiteMode.Strict
    //     // });
    //     
    //     return RedirectToAction("Index", "Home");
    // }

}