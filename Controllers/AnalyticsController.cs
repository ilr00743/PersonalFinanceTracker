using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinanceTracker.Models.DTO.Analytics;
using PersonalFinanceTracker.Services;

namespace PersonalFinanceTracker.Controllers;

[Authorize]
[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private const int MaxPeriodInDays = 730;
    private readonly AnalyticsService _analyticsService;

    public AnalyticsController(AnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }
    
    /// <summary>
    /// Get financial analytics summary for a specific period
    /// </summary>
    /// <param name="from">Start date (optional, default: first day of the current month)</param>
    /// <param name="to">End date (optional, default: today)</param>
    /// <returns>Analytics summary including income/expenses, category breakdown, trends, and budget overview</returns>
    
    /// <response code="200">Returns analytics summary</response>
    /// <response code="400">Invalid date range</response>
    /// <response code="401">Unauthorized (missing or invalid token)</response>
    
    [ProducesResponseType(typeof(AnalyticsSummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("summary")]
    public async Task<IActionResult> GetAnalyticsSummary([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
    {
        from ??= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        to ??= DateTime.Now;

        if (from > to)
        {
            return BadRequest("'from' date cannot be later than 'to' date");
        }

        if ((to - from).Value.Days > MaxPeriodInDays)
        {
            return BadRequest("Max period is 2 years");
        }

        var summary = await _analyticsService.GetSummaryAsync(from.Value, to.Value);

        return Ok(summary);
    }
}