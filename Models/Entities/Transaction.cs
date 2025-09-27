using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PersonalFinanceTracker.Attributes;

namespace PersonalFinanceTracker.Models.Entities;

public class Transaction
{
    public int Id { get; init; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [NotAllowedFutureDate]
    public DateTime Date { get; set; }
    
    [MaxLength(200)]
    public required string Description { get; set; }
    
    public required int CategoryId { get; set; }
    
    public Category Category { get; set; } = null!;

    public int UserId { get; set; }
}