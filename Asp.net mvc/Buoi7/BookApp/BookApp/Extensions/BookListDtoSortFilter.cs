using BookApp.Models;

namespace BookApp.Extensions;

public enum OrderByOptions { SimpleOrder = 0, ByVotes, ByPublicationDate, ByPriceLowestFirst }
public enum BooksFilterBy { NoFilter = 0, ByVotes, ByPublicationYear }

public static class BookListDtoSortFilter
{
    public static IQueryable<BookListDto> OrderBooksBy(
        this IQueryable<BookListDto> books, OrderByOptions orderBy) => orderBy switch
        {
            OrderByOptions.SimpleOrder => books.OrderByDescending(x => x.Id),
            OrderByOptions.ByVotes => books.OrderByDescending(x => x.AverageRating),
            OrderByOptions.ByPublicationDate => books.OrderByDescending(x => x.PublishedOn),
            OrderByOptions.ByPriceLowestFirst => books.OrderBy(x => x.Price),
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy))
        };

    public static IQueryable<BookListDto> FilterBooksBy(
        this IQueryable<BookListDto> books, BooksFilterBy filterBy, string filterValue)
    {
        if (string.IsNullOrWhiteSpace(filterValue)) return books;

        var today = DateOnly.FromDateTime(DateTime.Today);

        return filterBy switch
        {
            BooksFilterBy.NoFilter => books,
            BooksFilterBy.ByVotes => books.Where(x => x.AverageRating >= int.Parse(filterValue)),
            BooksFilterBy.ByPublicationYear => filterValue == "0"
                ? books.Where(x => x.PublishedOn > today)
                : books.Where(x => x.PublishedOn.Year == int.Parse(filterValue) && x.PublishedOn <= today),
            _ => throw new ArgumentOutOfRangeException(nameof(filterBy))
        };
    }
}