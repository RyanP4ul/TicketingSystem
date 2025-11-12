using LanBasedHelpDeskTickingSystem.Entities.Models;

namespace LanBasedHelpDeskTickingSystem.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    System.DateTimeOffset GetExpiry();
}