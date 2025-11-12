using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LanBasedHelpDeskTickingSystem.Entities.Enums;

namespace LanBasedHelpDeskTickingSystem.Entities.Models;

public class User
{
    
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required, MaxLength(320)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required] [Column("password")] public string PasswordHash { get; set; } = string.Empty;
    
    [Column("remember_me")]
    public string? RememberMe { get; set; } = string.Empty;
    
    // [Required, MaxLength(15)]
    // [Column("first_name")]
    // public string FirstName { get; set; } = string.Empty;
    //
    // [Required, MaxLength(15)]
    // [Column("middle_name")]
    // public string MiddleName { get; set; } = string.Empty;
    //
    // [Required, MaxLength(15)]
    // [Column("last_name")]
    // public string LastName { get; set; } = string.Empty;

    [Column("roles")]
    public UserRole Roles { get; set; } = UserRole.User;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}