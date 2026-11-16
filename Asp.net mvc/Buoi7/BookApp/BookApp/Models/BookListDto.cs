namespace BookApp.Models
{
    public record BookListDto(
    int Id,
    string Title,
    DateOnly PublishedOn,
    decimal Price,
    decimal? PromotionPrice,
    string? PromotionText,
    IReadOnlyList<string> AuthorNames, //Interface chỉ đọc k xóa hay thêm dc
    int ReviewCount,
    decimal? AverageRating);
}
