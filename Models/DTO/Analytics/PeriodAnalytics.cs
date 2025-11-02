namespace PersonalFinanceTracker.Models.DTO.Analytics;

public class PeriodAnalytics
{
    public DateTime PeriodStart { get; set; }
    public string PeriodLabel { get; set; }
    public decimal Income { get; set; }
    public decimal Expenses { get; set; }
    public decimal NetBalance { get; set; }
}