using BookApp.Data;
using BookApp.Models;
using BookApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Controllers;

public class BooksController(BookDbContext db, BookListService listService) : Controller
{
    // GET /Books?FilterBy=ByVotes&FilterValue=3&OrderBy=ByVotes&PageNum=1
    public async Task<IActionResult> Index(BookFilterOptions options)
    {
        var books = await listService.SortFilterPageAsync(options);
        ViewBag.Options = options;
        return View(books);
    }
    // GET /Books/Details/3
    public async Task<IActionResult> Details(int id)
    {
        var book = await db.Books
            .Include(b => b.Promotion)
            .Include(b => b.Reviews)
            .Include(b => b.BookAuthors.OrderBy(ba => ba.Order))
                .ThenInclude(ba => ba.Author)
            .AsNoTracking()
            .AsSplitQuery() // nhiều collection => tách câu SQL
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book is null) return NotFound();

        ViewBag.AllAuthors = await db.Authors
            .AsNoTracking()
            .OrderBy(a => a.Name)
            .ToListAsync();

        return View(book);
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddReview(int bookId, string voterName, int numStars, string? comment)
    {
        if (!await db.Books.AnyAsync(b => b.Id == bookId))
            return NotFound();

        // Cách hiệu quả nhất: thêm thẳng vào DbSet của phía "Nhiều", chỉ 1 câu INSERT
        db.Reviews.Add(new Review
        {
            BookId = bookId,
            VoterName = voterName,
            NumStars = Math.Clamp(numStars, 1, 5),
            Comment = comment
        });

        await db.SaveChangesAsync();
        TempData["Msg"] = "Cảm ơn bạn đã đánh giá!";

        return RedirectToAction(nameof(Details), new { id = bookId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(int reviewId, int bookId)
    {
        var review = await db.Reviews.FindAsync(reviewId);
        if (review is not null)
        {
            db.Reviews.Remove(review);
            await db.SaveChangesAsync();
            TempData["Msg"] = "Đã xóa đánh giá.";
        }

        return RedirectToAction(nameof(Details), new { id = bookId });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePromotion(int bookId, decimal? newPrice, string? promotionalText)
    {
        var book = await db.Books
            .Include(b => b.Promotion) // BẮT BUỘC: nạp khuyến mãi hiện có
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book is null) return NotFound();

        if (newPrice is null)
        {
            book.Promotion = null; // gỡ khuyến mãi => DELETE dòng PriceOffers
            TempData["Msg"] = "Đã gỡ khuyến mãi.";
        }
        else
        {
            book.Promotion = new PriceOffer // thay khuyến mãi cũ
            {
                NewPrice = newPrice.Value,
                PromotionalText = promotionalText ?? "Khuyến mãi"
            };
            TempData["Msg"] = "Đã cập nhật khuyến mãi.";
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = bookId });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeAuthors(int bookId, int[] authorIds)
    {
        var book = await db.Books
            .Include(b => b.BookAuthors) // nạp danh sách tác giả hiện tại
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book is null) return NotFound();

        book.BookAuthors.Clear(); // bỏ toàn bộ tác giả cũ

        byte order = 0;

        // Xử lý trường hợp authorIds bị null (người dùng bỏ chọn tất cả)
        if (authorIds != null)
        {
            foreach (var authorId in authorIds) // thứ tự do người dùng chọn
            {
                book.BookAuthors.Add(new BookAuthor { AuthorId = authorId, Order = order++ });
            }
        }

        await db.SaveChangesAsync(); // EF sinh DELETE + INSERT trên bảng BookAuthor
        TempData["Msg"] = "Đã cập nhật danh sách tác giả.";

        return RedirectToAction(nameof(Details), new { id = bookId });
    }
    // Xóa mềm: MỘT câu UPDATE, không tải thực thể về
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SoftDelete(int id)
    {
        var rows = await db.Books
            .Where(b => b.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.SoftDeleted, true));

        TempData["Msg"] = rows == 1 ? "Đã chuyển sách vào thùng rác." : "Không tìm thấy sách.";
        return RedirectToAction(nameof(Index));
    }

    // Thùng rác: phải bỏ qua bộ lọc toàn cục mới nhìn thấy
    public async Task<IActionResult> Trash()
    {
        var deleted = await db.Books
            .IgnoreQueryFilters(["SoftDelete"]) // EF Core 10: bỏ qua đúng bộ lọc cần bỏ
            .Where(b => b.SoftDeleted)
            .AsNoTracking()
            .ToListAsync();

        return View(deleted);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        await db.Books
            .IgnoreQueryFilters(["SoftDelete"])
            .Where(b => b.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.SoftDeleted, false));

        TempData["Msg"] = "Đã khôi phục sách.";
        return RedirectToAction(nameof(Trash));
    }

    // Xóa vĩnh viễn: xóa dữ liệu liên quan rồi xóa sách, trong MỘT transaction
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> HardDelete(int id)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Reviews.Where(r => r.BookId == id).ExecuteDeleteAsync();
            await db.Books
                .IgnoreQueryFilters(["SoftDelete"])
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();

            await tx.CommitAsync();
            TempData["Msg"] = "Đã xóa vĩnh viễn.";
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return RedirectToAction(nameof(Trash));
    }
}