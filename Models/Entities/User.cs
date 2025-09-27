using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models.Entities;

public class User
{
    public int Id { get; set; }
        
    [EmailAddress, MaxLength(100)]
    public required string Email { get; set; }
    
    public required string PasswordHash { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Category> Categories { get; set; } = new();
    public List<Transaction> Transactions { get; set; } = new();
}