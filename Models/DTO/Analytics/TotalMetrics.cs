namespace PersonalFinanceTracker.Models.DTO.Analytics;

public record TotalMetrics(
    decimal TotalIncome,
    decimal TotalExpenses,
    decimal Balance,
    decimal SavingsRate
    );