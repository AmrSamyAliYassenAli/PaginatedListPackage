using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace PaginatedListApp;
public class PaginatedList<T>
{
    /// <summary>
    /// Gets or Sets Index of Page
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// Gets or Sets TotalPages
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Gest or Sets TotalCount
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Flag for HasPreviousPage
    /// </summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>
    /// Flag for HasNextPage
    /// </summary>
    public bool HasNextPage => PageIndex < TotalPages;

    /// <summary>
    /// Gets or Sets Items
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Empty Constractor for Mapping
    /// </summary>
    public PaginatedList()
    {

    }

    /// <summary>
    /// Initating Private Constractor
    /// </summary>
    /// <param name="items"></param>
    /// <param name="count"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalCount = count;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
        Items.AddRange(items);
    }

    /// <summary>
    /// Factory Class to Create Objects of PaginatedList
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Create Paginated List
        /// </summary>
        /// <param name="pageIndex"> Index of Page Selected </param>
        /// <param name="pageSize"> Number of Items per Page </param>
        /// <param name="source"> Query </param>
        /// <param name="expression"> PredicateBuilder </param>
        /// <returns> PaginatedList </returns>
        public static async Task<PaginatedList<T>> CreateAsync(int pageIndex, int pageSize,
            IQueryable<T> source, ExpressionStarter<T>? expression = null)
        {
            if (expression is not null && expression.IsStarted)
                source = source.Where(expression);
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}