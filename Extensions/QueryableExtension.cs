using Microsoft.EntityFrameworkCore;
using PersonalFinanceTracker.Models.Entities;

namespace PersonalFinanceTracker.Extensions;

public static class QueryableExtension
{
    public static async Task<PagedList<T>> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedList<T>(items, count, items.Count, pageNumber, pageSize);
    }
}