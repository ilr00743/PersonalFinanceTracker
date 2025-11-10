using System.Collections;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO.Analytics;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class AnalyticsService
{
    private readonly FinanceDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(FinanceDbContext dbContext, IUserService userService, ILogger<AnalyticsService> logger)
    {
        _dbContext = dbContext;
        _userService = userService;
        _logger = logger;
    }

    public async Task<AnalyticsSummaryResponse> GetSummaryAsync(DateTime from, DateTime to)
    {
        var userTransactions = await _dbContext.Transactions
            .Where(t => t.UserId == _userService.GetCurrentUserId() 
                        && t.Date >= from 
                        && t.Date <= to)
            .Include(t => t.Category)
            .ToListAsync();
        
        var totalMetrics = CalculateTotalMetrics(userTransactions);

        var incomeCategoriesBreakdown =
            GetCategoryBreakdown(userTransactions, CategoryType.Income, totalMetrics.TotalIncome);

        var expensesCategoriesBreakdown =
            GetCategoryBreakdown(userTransactions, CategoryType.Expense, totalMetrics.TotalExpenses);

        var trend = GetTrend(userTransactions, from, to);

        var budgetOverview = await GetBudgetOverviewAsync(from, to, userTransactions);

        var topFiveExpenses = GetTopExpenses(userTransactions);

        return new AnalyticsSummaryResponse
        {
            TotalMetrics = totalMetrics,
            TransactionCount = userTransactions.Count,
            IncomesByCategory = incomeCategoriesBreakdown,
            ExpensesByCategory = expensesCategoriesBreakdown,
            Trend = trend,
            BudgetOverview = budgetOverview,
            TopExpenses = topFiveExpenses
        };
    }

    private List<PeriodAnalytics> GetTrend(List<Transaction> transactions, DateTime from, DateTime to)
    {
        List<PeriodAnalytics> list;

        var shouldGroupByMonth = ShouldGroupByMonth(from, to);
        
        if (shouldGroupByMonth)
        {
            var groupedTransactions = transactions.GroupBy(t => new { t.Date.Month, t.Date.Year });

            list = groupedTransactions.Select(g =>
            {
                var income = g.Where(t => t.Category.Type == CategoryType.Income).Sum(t => t.Amount);
                var expenses = g.Where(t => t.Category.Type == CategoryType.Expense).Sum(t => t.Amount);

                return new PeriodAnalytics
                {
                    PeriodStart = new DateTime(g.Key.Year, g.Key.Month, 1),
                    PeriodLabel = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Income = income,
                    Expenses = expenses,
                    NetBalance = income - expenses
                };
            }).OrderBy(p => p.PeriodStart).ToList();
        }
        else
        {
            var groupedTransactions = transactions.GroupBy(t => t.Date.Date);

            list = groupedTransactions.Select(g =>
            {
                var income = g.Where(t => t.Category.Type == CategoryType.Income).Sum(t => t.Amount);
                var expenses = g.Where(t => t.Category.Type == CategoryType.Expense).Sum(t => t.Amount);

                return new PeriodAnalytics
                {
                    PeriodStart = g.Key,
                    PeriodLabel = $"{g.Key.Year}-{g.Key.Month:D2}-{g.Key.Day:D2}",
                    Income = income,
                    Expenses = expenses,
                    NetBalance = income - expenses
                };
            }).OrderBy(p => p.PeriodLabel).ToList();
        }
        
        FillTrendGaps(list, from, to, shouldGroupByMonth);

        return list;
    }

    private TotalMetrics CalculateTotalMetrics(List<Transaction> transactions)
    {
        var incomeTransactions = transactions.Where(t => t.Category.Type == CategoryType.Income);
        var expensesTransactions = transactions.Where(t => t.Category.Type == CategoryType.Expense);

        var totalIncome = incomeTransactions.Sum(t => t.Amount);
        var totalExpenses = expensesTransactions.Sum(t => t.Amount);

        var balance = totalIncome - totalExpenses;

        var savingsRate = totalIncome > 0 ? balance / totalIncome * 100 : 0;

        return new TotalMetrics(totalIncome, totalExpenses, balance, savingsRate);
    }

    private List<CategoryAnalytics> GetCategoryBreakdown(List<Transaction> transactions, CategoryType categoryType,
        decimal total)
    {

        var filteredTransactions = transactions.Where(t => t.Category.Type == categoryType);

        var categoryAnalytics = filteredTransactions.GroupBy(t => t.Category)
            .Select(g =>
            {
                var amount = g.Sum(t => t.Amount);
                
                return new CategoryAnalytics
                {
                    CategoryId = g.Key.Id,
                    CategoryName = g.Key.Name,
                    Amount = amount,
                    TransactionCount = g.Count(),
                    Percentage = total != 0 ? amount / total * 100 : 0
                };
            }).OrderByDescending(c => c.Amount);

        return categoryAnalytics.ToList();
    }

    private async Task<BudgetOverview> GetBudgetOverviewAsync(DateTime from, DateTime to, List<Transaction> transactions)
    {
        var budgets = await _dbContext.Budgets
            .Where(b => b.UserId == _userService.GetCurrentUserId())
            .Include(b => b.Category)
            .ToListAsync();

        var activeBudgets = new List<Budget>();

        foreach (var budget in budgets)
        {
            var startPeriod = new DateTime(budget.Year, (int)budget.Month, 1);
            var endPeriod = startPeriod.AddMonths(1).AddDays(-1);

            if (startPeriod <= to && endPeriod >= from)
            {
                activeBudgets.Add(budget);
            }
        }
        
        var budgetsWithSpent = activeBudgets.Select(b => new
        {
            Budget = b,
            Spent = transactions.Where(t => t.CategoryId == b.CategoryId &&
                                            t.Date.Year == b.Year &&
                                            t.Date.Month == (int)b.Month &&
                                            t.Category.Type == CategoryType.Expense).Sum(t => t.Amount)
        }).ToList();

        var totalLimit = budgetsWithSpent.Sum(b => b.Budget.Limit);
        var totalSpent = budgetsWithSpent.Sum(b => b.Spent);

        var overview = new BudgetOverview
        {
            AllBudgetsCount = budgets.Count,
            ActiveBudgetsCount = activeBudgets.Count,
            OverBudgetCount = budgetsWithSpent.Count(b => b.Spent > b.Budget.Limit),
            TotalLimit = totalLimit,
            TotalSpent = totalSpent,
            AverageUtilization = totalLimit > 0 ? (totalSpent / totalLimit) * 100 : 0
        };
        
        return overview;
    }

    private List<TopTransactionDto> GetTopExpenses(List<Transaction> transactions)
    {
        var topExpenses = transactions
            .Where(t => t.Category.Type == CategoryType.Expense)
            .OrderByDescending(t => t.Amount)
            .Take(5)
            .Select(t => new TopTransactionDto
            {
                TransactionId = t.Id,
                Amount = t.Amount,
                Description = t.Description,
                CategoryName = t.Category.Name,
                Date = t.Date
            }).ToList();
        
        return topExpenses;
    }
    
    private void FillTrendGaps(List<PeriodAnalytics> trend, DateTime from, DateTime to, bool isMonthly)
    {
        if (isMonthly)
        {
            var dates = new List<DateTime>();

            var currentMonth = new DateTime(from.Year, from.Month, 1);

            var toDate = new DateTime(to.Year, to.Month, 1);
            
            while (currentMonth <= toDate)
            {
                dates.Add(currentMonth);
                currentMonth = currentMonth.AddMonths(1);
            }

            foreach (var date in dates)
            {
                if (trend.Any(p => p.PeriodStart == date)) continue;
                
                trend.Add(new PeriodAnalytics
                {
                    PeriodStart = new DateTime(date.Year, date.Month, 1),
                    PeriodLabel = $"{date.Year}-{date.Month:D2}",
                    Income = 0,
                    Expenses = 0,
                    NetBalance = 0
                });
            }
        }
        else
        {
            var daysCount = (to - from).Days + 1;
            var days = Enumerable.Range(0, daysCount).Select(i => from.AddDays(i));

            foreach (var day in days)
            {
                if (trend.Any(p => p.PeriodStart == day)) continue;
                
                trend.Add(new PeriodAnalytics
                {
                    PeriodStart = day,
                    PeriodLabel = $"{day.Year}-{day.Month:D2}-{day.Day:D2}",
                    Income = 0,
                    Expenses = 0,
                    NetBalance = 0
                });
            }
            
            trend.Sort((a, b) => a.PeriodStart.CompareTo(b.PeriodStart));
        }
    }
    
    private bool ShouldGroupByMonth(DateTime from, DateTime to)
    {
        var dateDifference = ((to.Year - from.Year) * 12) + (to.Month - from.Month);

        return dateDifference > 0;
    }
}