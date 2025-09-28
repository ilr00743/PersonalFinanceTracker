using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models.DTO;

public class RegisterDto
{
    [MaxLength(100), EmailAddress]
    public required string Email { get; set; }
    
    [MinLength(8)]
    public required string Password { get; set; }
    
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }
}