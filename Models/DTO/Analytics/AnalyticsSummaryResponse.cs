namespace PersonalFinanceTracker.Models.DTO.Analytics;

public class AnalyticsSummaryResponse
{
    public TotalMetrics TotalMetrics { get; set; }
    
    public int TransactionCount { get; set; }
    public List<CategoryAnalytics> IncomesByCategory { get; set; } = new();
    public List<CategoryAnalytics> ExpensesByCategory { get; set; } = new();
    public List<PeriodAnalytics> Trend { get; set; } = new();
    public BudgetOverview BudgetOverview { get; set; }
    public List<TopTransactionDto> TopExpenses { get; set; } = new();
}