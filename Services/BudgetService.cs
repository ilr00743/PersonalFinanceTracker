using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Data;
using PersonalFinanceTracker.Models.DTO.Budget;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Services;

public class BudgetService
{
    private readonly FinanceDbContext _dbContext;
    private readonly ILogger<BudgetService> _logger;
    private readonly IUserService _userService;

    public BudgetService(FinanceDbContext dbContext, ILogger<BudgetService> logger, IUserService userService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userService = userService;
    }

    public async Task<BudgetResponse?> GetBudget(int id)
    {
        var budget = await _dbContext.Budgets
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == _userService.GetCurrentUserId());

        if (budget is null)
        {
            return null;
        }

        var spent = await CalculateSpent(budget.CategoryId, budget.Month, budget.Year);
        var remaining = budget.Limit - spent;
        var percentageUsage = budget.Limit > 0 ? (spent / budget.Limit) * 100 : 0;

        return new BudgetResponse
        {
            Id = budget.Id,

            CategoryId = budget.CategoryId,
            CategoryName = budget.Category.Name,
            CategoryType = budget.Category.Type,
            CreatedAt = budget.CreatedAt,

            Limit = budget.Limit,

            Month = budget.Month,
            Year = budget.Year,

            Spent = spent,
            Remaining = remaining,
            PercentageUsed = percentageUsage,
            IsOverBudget = spent > budget.Limit
        };
    }

    public async Task<IEnumerable<BudgetResponse>> GetBudgets()
    {
        var userId = _userService.GetCurrentUserId();
        
        var budgets = await _dbContext.Budgets
            .Where(b => b.UserId == userId)
            .Include(budget => budget.Category)
            .ToListAsync();

        var budgetsDto = await Task.WhenAll(budgets.Select(async (b) =>
        {
            var spent = await CalculateSpent(b.CategoryId, b.Month, b.Year);

            return new BudgetResponse
            {
                Id = b.Id,

                CategoryId = b.CategoryId,
                CategoryName = b.Category.Name,
                CategoryType = b.Category.Type,
                CreatedAt = b.CreatedAt,

                Limit = b.Limit,

                Month = b.Month,
                Year = b.Year,

                Spent = spent,
                Remaining = b.Limit - spent,
                PercentageUsed = b.Limit > 0 ? (spent / b.Limit) * 100 : 0,
                IsOverBudget = spent > b.Limit
            };
        }));

        return budgetsDto;
    }

    public async Task CreateBudget(CreateBudgetDto budget)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateBudget(int id, UpdateBudgetDto updatedBudget)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteBudget(int id)
    {
        var budget = await _dbContext.Budgets.FindAsync(id);

        if (budget is null)
        {
            _logger.LogWarning("Budget {id} not found for user {userId}", id, _userService.GetCurrentUserId());
            return;
        }

        _dbContext.Budgets.Remove(budget);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Budget {id} deleted for user {userId}", id, _userService.GetCurrentUserId());
    }

    private async Task<decimal> CalculateSpent(int categoryId, Month month, int year)
    {
        return await _dbContext.Transactions
            .Where(t => t.CategoryId == categoryId
                        && t.Date.Month == (int)month
                        && t.Date.Year == year
                        && t.UserId == _userService.GetCurrentUserId())
            .SumAsync(t => t.Amount);
    }
}