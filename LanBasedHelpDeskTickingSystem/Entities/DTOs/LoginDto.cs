using System.ComponentModel.DataAnnotations;

namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Username or Email is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    // [RegularExpression("^[A-Za-z]+$", ErrorMessage = "First name must contain only letters")]
    public required string UsernameOrEmail { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    public required string Password { get; set; }
}