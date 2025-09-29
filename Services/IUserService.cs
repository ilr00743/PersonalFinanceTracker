namespace PersonalFinanceTracker.Services;

public interface IUserService
{
    int GetCurrentUserId();
    string GetCurrentUserEmail();
    bool IsUserAuthenticated();
}