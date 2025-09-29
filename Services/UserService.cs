using System.Security.Claims;

namespace PersonalFinanceTracker.Services;

public class UserService : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public int GetCurrentUserId()
    {
        var nameIdentifier = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

        if (nameIdentifier == null)
        {
            throw new UnauthorizedAccessException("No user id claim found");
        }
        
        return int.Parse(nameIdentifier.Value);
    }

    public string GetCurrentUserEmail()
    {
        var email = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email);

        if (email == null)
        {
            throw new UnauthorizedAccessException("No user email claim found");
        }
        
        return email.Value;
    }

    public bool IsUserAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}