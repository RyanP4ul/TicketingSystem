using Microsoft.EntityFrameworkCore;

namespace LanBasedHelpDeskTickingSystem.Libs;

public class PaginatedList<T> : List<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }
    public int TotalItems { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalItems = count;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
    {
        // Count total items in DB (this executes a SQL COUNT query)
        var count = await source.CountAsync();

        // Only fetch the current page items (this executes a SQL SELECT with OFFSET/FETCH)
        var items = await source
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}