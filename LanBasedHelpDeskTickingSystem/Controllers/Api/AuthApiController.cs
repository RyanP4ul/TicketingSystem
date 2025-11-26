using System.Security.Claims;
using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Repository.Interfaces;
using LanBasedHelpDeskTickingSystem.Services;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api;

using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LanBasedHelpDeskTickingSystem.Services;

[Route("api/auth")]
public class AuthApiController(IUserService userService, IUserRepository userRepository, IJwtService jwtService) : ControllerBase
{
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }

        var result = await userService.CreateAsync(model.Username, model.Email, model.Password);
        
        if (result == null) return Conflict(ApiResponse<string>.Error("Email already exists"));
        if (!result.Success) return BadRequest(ApiResponse<string>.Error(result.Message ?? "Unknown error"));
        if (result.Data == null) return BadRequest(ApiResponse<string>.Error("User creation failed"));
        
        var user = result.Data;
        
        var token = jwtService.GenerateToken(user);
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(new AuthResponseDto
        {
            Token = token,
            ExpiresAt = jwtService.GetExpiry().ToUnixTimeSeconds(),
            Username = user.Username,
            Roles = user.Roles.ToString()
        });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        if (User.Identity?.IsAuthenticated == true) return BadRequest(ApiResponse<string>.Error("Already authenticated"));
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                );

            return BadRequest(new { success = false, errors });
        }
        
        var user = await userService.AuthenticateAsync(model.UsernameOrEmail, model.Password);
        
        if (user == null) return Unauthorized(ApiResponse<string>.Error("Invalid credentials"));
        
        var token = jwtService.GenerateToken(user);
        
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict
        });
        
        return Ok(ApiResponse<string>.Ok("success"));
    }
    
    [HttpGet("login/google")]
    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action(nameof(GoogleResponse), "AuthApi", null, Request.Scheme);
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }
    
    [HttpGet("login/google/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

        if (!result.Succeeded || result.Principal == null) return BadRequest("External login failed.");

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var name = result.Principal.FindFirstValue(ClaimTypes.Name);
        
        if (email == null) return BadRequest("Email claim not found.");
        if (name == null) return BadRequest("Name claim not found.");
        
        var user = await userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            user = await userRepository.CreateUserByGoogleAsync(name, email);   
            if (user == null) return BadRequest("User creation failed.");
        }
        
        var token = jwtService.GenerateToken(user);
        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return RedirectToAction("Login", "Auth", "google_success");
    }
    
}