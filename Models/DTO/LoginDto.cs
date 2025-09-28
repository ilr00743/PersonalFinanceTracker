using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models.DTO;

public class LoginDto
{
    [EmailAddress]
    public required string Email { get; set; }
    
    public required string Password { get; set; }
}