using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services.JWT;

public interface IJwtService
{
    public string GenerateToken(User user);
    public DateTime GetTokenExpiration();
}