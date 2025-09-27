using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models.Entities;

public class Category
{
    public int Id { get; init; }
    
    [MaxLength(50)]
    public required string Name { get; set; }
    
    [MaxLength(200)]
    public string? Description { get; set; }
    public CategoryType Type { get; set; }

    public int UserId { get; set; }
}

public enum CategoryType
{
    None = 0,
    Income = 1,
    Expense = 2
}