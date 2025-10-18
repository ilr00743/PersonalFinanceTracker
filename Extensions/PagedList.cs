namespace PersonalFinanceTracker.Extensions;

public class PagedList<T> : List<T>
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalItemsAmount { get; set; }
    public int ItemsAmountOnCurrentPage { get; set; }

    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    public PagedList(List<T> items, int totalItemsAmount, int itemsAmountOnCurrentPage, int currentPage, int pageSize)
    {
        TotalItemsAmount = totalItemsAmount;
        CurrentPage = currentPage;
        TotalPages = (int)Math.Ceiling(totalItemsAmount / (double)pageSize);
        PageSize = pageSize;
        ItemsAmountOnCurrentPage = itemsAmountOnCurrentPage;
        
        AddRange(items);
    }

    public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return await Task.Run(() => new PagedList<T>(items, count, items.Count, pageNumber, pageSize));
    }
}