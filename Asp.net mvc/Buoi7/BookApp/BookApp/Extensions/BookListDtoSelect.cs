using BookApp.Models;
namespace BookApp.Extensions;

public static class BookListDtoSelect
{
    public static IQueryable<BookListDto> MapBookToDto(this IQueryable<Book> books) =>
        books.Select(b => new BookListDto(
            b.Id,
            b.Title,
            b.PublishedOn,
            b.Price,
            b.Promotion != null ? b.Promotion.NewPrice : (decimal?)null,       // Sửa cho an toàn
            b.Promotion != null ? b.Promotion.PromotionalText : null,          // Sửa cho an toàn

            // [SỬA 1] Bỏ string.Join, dùng ToList() để trả về IReadOnlyList<string>
            b.BookAuthors.OrderBy(ba => ba.Order)
                .Select(ba => ba.Author.Name)
                .ToList(),

            b.Reviews.Count(),

            // [SỬA 2] Đổi double? thành decimal? cho khớp với DTO
            b.Reviews.Select(r => (decimal?)r.NumStars).Average()
        ));
}