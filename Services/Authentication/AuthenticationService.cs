using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private const int WorkFactor = 12;
    
    private readonly FinanceDbContext _dbContext;
    private readonly ILogger<AuthenticationService> _logger;
    
    public AuthenticationService(FinanceDbContext dbContext, ILogger<AuthenticationService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<User> RegisterUserAsync(RegisterDto dto)
    {
        _logger.LogInformation("Attempting to register user with email {email}", dto.Email);
        
        if (await _dbContext.Users.AnyAsync(u => u.Email == dto.Email))
        {
            _logger.LogWarning("User with this email already exists");
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
        
        _logger.LogInformation("User with email {email} registered", newUser.Email);
        
        return newUser;
    }

    public async Task<User?> ValidateUserCredentialsAsync(LoginDto dto)
    {
        _logger.LogInformation("Validating user credentials with email {email}", dto.Email);
        
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user is null)
        {
            _logger.LogWarning("User not found");
            return null;
        }

        if (!IsPasswordVerified(dto.Password, user.PasswordHash))
        {
            _logger.LogWarning("Invalid password for user");
            return null;
        }
        
        _logger.LogInformation("User {UserId} credentials are valid", user.Id);

        return user;
        
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