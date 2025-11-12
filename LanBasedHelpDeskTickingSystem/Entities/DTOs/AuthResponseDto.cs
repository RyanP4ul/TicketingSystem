namespace LanBasedHelpDeskTickingSystem.Entities.DTOs;

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required long ExpiresAt { get; set; }
    public required string Username { get; set; }
    public required string Roles { get; set; }
}