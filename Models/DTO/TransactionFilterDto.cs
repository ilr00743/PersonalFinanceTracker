namespace PersonalFinanceTracker.Models.DTO;

public class TransactionFilterDto
{
    public DateTime? From { get; set; } = null;
    
    public DateTime? To { get; set; } = null;
    
    public int? CategoryId { get; set; } = null;

    public decimal? MinAmount { get; set; } = null;

    public decimal? MaxAmount { get; set; } = null;
}