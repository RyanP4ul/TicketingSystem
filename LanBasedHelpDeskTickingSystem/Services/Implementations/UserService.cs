using LanBasedHelpDeskTickingSystem.Data;
using LanBasedHelpDeskTickingSystem.Entities.DTOs;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Entities.Responses;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Services.Implementations;

public class UserService(AppDbContext db, IPasswordHasher<User> hasher) : IUserService
{
    
    public async Task<ApiResponse<User>?> CreateAsync(string username, string email, string password)
    {
        var exists = await db.SetEntity<User>().CountAsync(u => u.Email == email) > 0;
        if (exists) return ApiResponse<User>.Error("Email already taken!");

        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var user = new User
            {
                Username = username,
                Email = email,
            };

            user.PasswordHash = hasher.HashPassword(user, password);

            db.SetEntity<User>().Add(user);

            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponse<User>.Ok(user);
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            return ApiResponse<User>.Error("Database error occurred while creating the user.");
        }
    }

    public async Task<User?> AuthenticateAsync(string usernameOrEmail, string password)
    {
        var user = await db.SetEntity<User>()
            .Where(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail)
            .FirstOrDefaultAsync();

        if (user == null) return null;

        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success ? user : null;
    }

    public async Task<User?> GetByIdAsync(int id) => await db.SetEntity<User>().FindAsync(id);
}