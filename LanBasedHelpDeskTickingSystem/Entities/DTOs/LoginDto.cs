using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public required string UsernameOrEmail { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}