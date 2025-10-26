using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Models.DTO.Budget;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBudget(int id)
    {
        var budget = await _budgetService.GetBudget(id);

        if (budget is null)
        {
            return NotFound();
        }

        return Ok(budget);
    }

    [HttpGet]
    public async Task<IActionResult> GetBudgets()
    {
        var budgets = await _budgetService.GetBudgets();
        return Ok(budgets);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget(CreateBudgetDto dto)
    {
        await _budgetService.CreateBudget(dto);
        return Created("", dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBudget(int id, UpdateBudgetDto dto)
    {
        await _budgetService.UpdateBudget(id, dto);

        var updatedBudget = await _budgetService.GetBudget(id);
        return Ok(updatedBudget);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBudget(int id)
    {
        await _budgetService.DeleteBudget(id);
        return NoContent();
    }
}