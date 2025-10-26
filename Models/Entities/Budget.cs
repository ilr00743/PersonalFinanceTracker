using System.ComponentModel.DataAnnotations;

namespace PersonalFinanceTracker.Models.Entities;

public class Budget
{
    public int Id { get; set; }
    
    public required int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    
    [Range(1, 1000000)]
    public decimal Limit { get; set; }
    
    [EnumDataType(typeof(Month))]
    public Month Month { get; set; }
    
    [Range(2025, 2100)]
    public int Year { get; set; }

    public DateTime CreatedAt { get; init; }
    
    public required int UserId { get; set; }
}

public enum Month
{
    January = 1,
    February = 2,
    March = 3,
    April = 4,
    May = 5,
    June = 6,
    July = 7,
    August = 8,
    September = 9,
    October = 10,
    November = 11,
    December = 12
}