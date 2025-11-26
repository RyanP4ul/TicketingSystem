using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;

namespace LanBasedHelpDeskTickingSystem.Services.Interfaces;

public interface IUserService
{
    Task<ApiResponse<User>?> CreateAsync(string username, string email, string password);
    Task<User?> AuthenticateAsync(string usernameOrEmail, string password);
    Task<User?> GetByIdAsync(int id);
}