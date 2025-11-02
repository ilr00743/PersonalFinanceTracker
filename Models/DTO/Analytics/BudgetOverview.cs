namespace PersonalFinanceTracker.Models.DTO.Analytics;

public class BudgetOverview
{
    public int AllBudgetsCount { get; set; }
    public int ActiveBudgetsCount { get; set; }
    public int OverBudgetCount { get; set; }
    public decimal TotalLimit { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal AverageUtilization { get; set; }
}