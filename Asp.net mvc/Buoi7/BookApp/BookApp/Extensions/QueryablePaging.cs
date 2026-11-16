namespace BookApp.Extensions;

public static class QueryablePaging
{
    public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageNumZeroStart, int pageSize)
    {
        if (pageSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");

        if (pageNumZeroStart < 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumZeroStart), "Page number cannot be negative.");

        // Nếu pageNumZeroStart = 0 (trang đầu), bỏ qua Skip
        if (pageNumZeroStart > 0)
            query = query.Skip(pageNumZeroStart * pageSize); // OFFSET

        return query.Take(pageSize); // FETCH NEXT
    }
}