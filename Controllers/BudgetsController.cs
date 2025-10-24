using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Controllers;

[Authorize]
[ApiController]
[Route("api/budgets")]
public class BudgetsController : ControllerBase
{
    private readonly BudgetService _budgetService;

    public BudgetsController(BudgetService budgetService)
    {
        _budgetService = budgetService;
    }
    
    
}