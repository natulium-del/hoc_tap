using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApp.Data;
using BookApp.Models;

namespace BookApp.Controllers;

public class LabController(BookDbContext db) : Controller
{
    // GET /Lab/Q1
    public async Task<IActionResult> Q1()
    {
        var sb = new StringBuilder();

        // (1) Dựng truy vấn – CHƯA có câu SQL nào được gửi đi
        IQueryable<Book> query = db.Books.Where(b => b.Price < 300_000);
        sb.AppendLine("Đã dựng xong IQueryable – hãy nhìn Console: chưa có SQL.");

        // (2) Ghép thêm điều kiện – vẫn chưa chạy
        query = query.OrderBy(b => b.Title);

        // (3) Lệnh thực thi -> SQL được gửi tới CSDL
        var books = await query.ToListAsync();
        sb.AppendLine($"Số sách dưới 300.000đ: {books.Count}");

        // So sánh các lệnh lấy một phần tử
        var byKey = await db.Books.FindAsync(3); // tìm trong bộ nhớ trước
        var first = await db.Books.FirstOrDefaultAsync(b => b.Title.Contains("EF")); // null nếu không có
        var count = await db.Books.CountAsync(); // SELECT COUNT(*)
        var any = await db.Books.AnyAsync(b => b.Price > 1_000_000); // EXISTS

        sb.AppendLine($"FindAsync(3): {byKey?.Title}");
        sb.AppendLine($"FirstOrDefaultAsync: {first?.Title}");
        sb.AppendLine($"CountAsync: {count} – AnyAsync(>1 triệu): {any}");

        // Chỉ lấy cột cần dùng (projection)
        var titles = await db.Books.Select(b => new { b.Id, b.Title }).ToListAsync();
        sb.AppendLine($"Projection trả về {titles.Count} dòng, chỉ 2 cột.");

        return Content(sb.ToString());
    } 

    // GET /Lab/Q2
    public async Task<IActionResult> Q2()
    {
        var sb = new StringBuilder();

        // ---------- A. Cách SAI: truy vấn trong vòng lặp => lỗi N + 1 ----------
        Console.WriteLine("=== A. Vòng lặp truy vấn (N + 1) ===");
        var books = await db.Books.ToListAsync(); // 1 câu SQL
        foreach (var b in books)
        {
            var reviews = await db.Reviews.Where(r => r.BookId == b.Id).ToListAsync(); // + 1 câu / sách
            sb.AppendLine($"A | {b.Title}: {reviews.Count} đánh giá");
        }

        // ---------- B. Eager loading ----------
        Console.WriteLine("=== B. Include ===");
        var withReviews = await db.Books
            .Include(b => b.Reviews)
            .Include(b => b.Promotion)
            .AsNoTracking()
            .ToListAsync(); // 1 câu SQL
        foreach (var b in withReviews)
            sb.AppendLine($"B | {b.Title}: {b.Reviews.Count} đánh giá, KM: {b.Promotion?.NewPrice}");

        // ---------- C. Projection ----------
        Console.WriteLine("=== C. Projection ===");
        var dto = await db.Books.Select(b => new
        {
            b.Title,
            ReviewCount = b.Reviews.Count(), // COUNT chạy trong SQL
            Average = b.Reviews.Select(r => (double?)r.NumStars).Average()
        }).ToListAsync(); // 1 câu SQL
        foreach (var x in dto)
            sb.AppendLine($"C | {x.Title}: {x.ReviewCount} đánh giá, TB {x.Average:0.0}");

        // ---------- D. Explicit loading ----------
        Console.WriteLine("=== D. Explicit loading ===");
        var book = await db.Books.FirstAsync(b => b.Id == 3);
        await db.Entry(book).Collection(b => b.Reviews).LoadAsync(); // câu SQL riêng
        var fiveStars = await db.Entry(book).Collection(b => b.Reviews)
            .Query().CountAsync(r => r.NumStars == 5);
        sb.AppendLine($"D | {book.Title}: {book.Reviews.Count} đánh giá, {fiveStars} đánh giá 5 sao");

        return Content(sb.ToString());
    }
}