using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LanBasedHelpDeskTickingSystem.Entities.Models;
using LanBasedHelpDeskTickingSystem.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace LanBasedHelpDeskTickingSystem.Services.Implementations;

public class JwtService : IJwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expireMinutes;

    public JwtService(IConfiguration config)
    {
        _key = config["Jwt:Key"];
        _issuer = config["Jwt:Issuer"];
        _audience = config["Jwt:Audience"];
        _expireMinutes = int.Parse(config["Jwt:ExpireMinutes"] ?? "60");
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Roles.ToString() ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_expireMinutes);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public DateTimeOffset GetExpiry()
    {
        return DateTimeOffset.UtcNow.AddMinutes(_expireMinutes);
    }
}