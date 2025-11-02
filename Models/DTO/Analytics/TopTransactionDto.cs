namespace PersonalFinanceTracker.Models.DTO.Analytics;

public class TopTransactionDto
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public string CategoryName { get; set; }
    public DateTime Date { get; set; }
}