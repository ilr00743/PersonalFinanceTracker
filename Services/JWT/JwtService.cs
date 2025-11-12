using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services.JWT;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    
    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("jwt_key")!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = GetTokenExpiration(),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "PersonalFinanceTracker",
            Audience = "PersonalFinanceTracker-Users",
            IssuedAt = DateTime.UtcNow
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        
        return tokenString;
    }

    public DateTime GetTokenExpiration()
    {
        var expirationHours = _configuration.GetSection("JwtSettings:ExpirationHours").Get<int>();
        return DateTime.UtcNow.AddHours(expirationHours);
    }
}