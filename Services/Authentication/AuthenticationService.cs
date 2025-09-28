using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private const int WorkFactor = 12;
    private readonly FinanceDbContext _dbContext;

    public AuthenticationService(FinanceDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<User> RegisterUserAsync(RegisterDto dto)
    {
        if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email))
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        var hash = GetPasswordHash(dto.Password);

        var newUser = new User
        {
            Email = dto.Email,
            PasswordHash = hash,
            CreatedAt = DateTime.UtcNow
        };
        
        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
        
        return newUser;
    }

    public async Task<User?> ValidateUserCredentialsAsync(LoginDto dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null)
        {
            return null;
        }
        
        return IsPasswordVerified(dto.Password, user.PasswordHash) ? user : null;
    }

    public string GetPasswordHash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
    }

    public bool IsPasswordVerified(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}