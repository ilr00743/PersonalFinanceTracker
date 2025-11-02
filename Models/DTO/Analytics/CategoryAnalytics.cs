namespace PersonalFinanceTracker.Models.DTO.Analytics;

public class CategoryAnalytics
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public int TransactionCount { get; set; }
}