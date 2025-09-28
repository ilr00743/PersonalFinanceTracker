namespace PersonalFinanceTracker.Models.DTO;

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public DateTime ExpiresAt { get; set; }
}