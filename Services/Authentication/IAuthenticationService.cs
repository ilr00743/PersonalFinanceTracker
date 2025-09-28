using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services.Authentication;

public interface IAuthenticationService
{
    Task<User> RegisterUserAsync(RegisterDto dto);
    Task<User?> ValidateUserCredentialsAsync(LoginDto dto);
    string GetPasswordHash(string password);
    bool IsPasswordVerified(string password, string hashedPassword);
}