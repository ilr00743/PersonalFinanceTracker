using System.ComponentModel.DataAnnotations;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Models.DTO.Budget;

public record BudgetResponse
{
    public int Id { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public CategoryType CategoryType { get; set; }

    public decimal Limit { get; set; }
    public Month Month { get; set; }
    public int Year { get; set; }

    public decimal Spent { get; set; }
    public decimal Remaining { get; set; }
    public decimal PercentageUsed { get; set; }
    public bool IsOverBudget { get; set; }
    
    public DateTime CreatedAt { get; set; }
}