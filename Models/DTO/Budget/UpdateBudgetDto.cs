using System.ComponentModel.DataAnnotations;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Models.DTO.Budget;

public record UpdateBudgetDto
{
    public int CategoryId { get; set; }
    
    [Range(1, 1000000)]
    public decimal Limit { get; set; }
    
    [EnumDataType(typeof(Month))]
    public Month Month { get; set; }
    
    [Range(2025, 2100)]
    public int Year { get; set; }
}