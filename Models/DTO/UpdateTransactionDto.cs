using System.ComponentModel.DataAnnotations;
using PersonalFinanceTracker.Attributes;

namespace PersonalFinanceTracker.Models.DTO;

public class UpdateTransactionDto
{
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
    
    [MaxLength(200)] 
    public required string Description { get; set; }
    
    [NotAllowedFutureDate]
    public DateTime Date { get; set; }
    
    public int CategoryId { get; set; }
    public int UserId { get; set; }
}