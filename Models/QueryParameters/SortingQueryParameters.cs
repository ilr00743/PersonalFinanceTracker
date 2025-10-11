namespace PersonalFinanceTracker.Models.QueryParameters;

public class SortingQueryParameters
{
    public string OrderBy { get; set; } = string.Empty;
    public bool ShouldOrderAscending { get; set; } = true;
}