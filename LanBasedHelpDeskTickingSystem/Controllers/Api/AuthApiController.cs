using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Services;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LanBasedHelpDeskTickingSystem.Controllers.Api;

using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LanBasedHelpDeskTickingSystem.Services;

[Route("api/auth")]
public class AuthApiController(IUserService userService, IJwtService jwtService) : ControllerBase
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
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return Ok(ApiResponse<string>.Ok("success"));
        // return RedirectToAction("Index", "Home");
    }
    // public async Task<IActionResult> Login([FromBody] LoginDto dto)
    // {
    //     if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
    //         return BadRequest("Provide username/email and password.");
    //
    //     var user = await userService.AuthenticateAsync(dto.UsernameOrEmail.Trim(), dto.Password);
    //     if (user == null) return Unauthorized("Invalid credentials.");
    //
    //     var token = jwtService.GenerateToken(user);
    //     return Ok(new AuthResponseDto
    //     {
    //         Token = token,
    //         ExpiresAt = jwtService.GetExpiry().ToUnixTimeSeconds(),
    //         Username = user.Username,
    //         Roles = user.Roles
    //     });
    // }
}