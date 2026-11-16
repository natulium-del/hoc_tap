using BookApp.Models;                 // Sử dụng các lớp thực thể (Book, Author, v.v.)
using Microsoft.EntityFrameworkCore;  // Sử dụng các hàm mở rộng của EF Core (AnyAsync, IgnoreQueryFilters...)

namespace BookApp.Data
{
    public class SeedData
    {
        // Hàm static (tĩnh) để tạo dữ liệu mẫu. 
        // Tham số: context (DbContext), bool _ (bỏ qua), ct (CancellationToken để hủy tác vụ nếu cần)
        public static async Task SeedAsync(BookDbContext context, bool _, CancellationToken ct)
        {
            // Ép kiểu context chung thành BookDbContext để truy cập được các bảng (DbSet)
            var db = (BookDbContext)context;

            // 1. KIỂM TRA DỮ LIỆU ĐÃ TỒN TẠI CHƯA
            // IgnoreQueryFilters(): Tạm bỏ qua bộ lọc xóa mềm (SoftDelete) để kiểm tra cả những sách đã bị xóa.
            // AnyAsync(): Trả về true nếu trong bảng Books có ít nhất 1 dòng.
            // Nếu đã có dữ liệu (true) -> return (dừng hàm, không tạo thêm dữ liệu mẫu nữa).
            if (await db.Books.IgnoreQueryFilters().AnyAsync(ct)) return;

            // 2. TẠO DANH SÁCH TÁC GIẢ
            // Tạo 4 tác giả mới (a1, a2, a3, a4). Đây là cú pháp khởi tạo đối tượng rút gọn (new()).
            Author a1 = new() { Name = "Nguyễn Văn A" }, a2 = new() { Name = "Trần Thị B" },
                   a3 = new() { Name = "Lê C" }, a4 = new() { Name = "Phạm D" };

            // 3. HÀM PHỤ TRỢ TẠO SÁCH (Local Function)
            // Hàm này giúp tạo nhanh một cuốn sách và gán tác giả cho nó, tránh viết code lặp lại.
            // Tham số params (Author a, byte order)[] authors: Cho phép truyền vào danh sách các cặp (tác giả, thứ tự).
            Book Make(string title, int y, int m, int d, decimal price, params (Author a, byte order)[] authors)
            {
                // Khởi tạo cuốn sách với các thông tin cơ bản
                var book = new Book { Title = title, PublishedOn = new DateOnly(y, m, d), Price = price };

                // Duyệt qua danh sách các cặp (tác giả, thứ tự) được truyền vào
                foreach (var (a, order) in authors)
                {
                    // Thêm một dòng vào bảng trung gian BookAuthor để liên kết Sách và Tác giả.
                    // Cột Order lưu thứ tự của tác giả (0 là tác giả chính, 1 là đồng tác giả...).
                    book.BookAuthors.Add(new BookAuthor { Author = a, Order = order });
                }
                return book; // Trả về cuốn sách đã tạo
            }

            // 4. TẠO CÁC CUỐN SÁCH SỬ DỤNG HÀM MAKEmm
            var b1 = Make("Lập trình C# 14 cơ bản", 2024, 5, 10, 250_000, (a1, 0)); // Sách 1: Do a1 viết
            var b2 = Make("ASP.NET Core MVC thực chiến", 2025, 9, 1, 380_000, (a2, 0), (a3, 1)); // Sách 2: Do a2 và a3 đồng viết
            var b3 = Make("EF Core 10 in Action", 2026, 1, 15, 350_000, (a1, 0)); // Sách 3
            var b4 = Make("Quantum Networking", 2026, 3, 1, 220_000, (a4, 0)); // Sách 4
            var b5 = Make("Clean Architecture cho .NET", 2023, 11, 20, 290_000, (a3, 0)); // Sách 5
            var b6 = Make("Sách cũ đã gỡ bán", 2019, 4, 1, 120_000, (a1, 0)); // Sách 6

            // Đánh dấu sách b6 là đã xóa mềm (SoftDeleted = true)
            // Mục đích: Để thử nghiệm xem bộ lọc toàn cục (Global Query Filter) có hoạt động đúng không.
            b6.SoftDeleted = true;

            // 5. THÊM KHUYẾN MÃI (QUAN HỆ 1-1)
            // Gán khuyến mãi cho một số sách. Khi thêm vào đối tượng sách, EF Core sẽ tự hiểu đây là quan hệ 1-1.
            b3.Promotion = new PriceOffer { NewPrice = 299_000, PromotionalText = "Ra mắt -15%" };
            b5.Promotion = new PriceOffer { NewPrice = 261_000, PromotionalText = "Sale" };
            b4.Promotion = new PriceOffer { NewPrice = 199_000, PromotionalText = "Đặt trước" };

            // 6. HÀM PHỤ TRỢ THÊM ĐÁNH GIÁ (Local Function)
            // Tương tự hàm Make, hàm này giúp thêm nhanh nhiều đánh giá cho 1 cuốn sách.
            void Vote(Book b, params (string name, int stars, string comment)[] rs)
            {
                // Duyệt qua danh sách các đánh giá
                foreach (var (name, stars, comment) in rs)
                {
                    // Thêm Review vào danh sách Reviews của sách (Quan hệ 1-N)
                    b.Reviews.Add(new Review { VoterName = name, NumStars = stars, Comment = comment });
                }
            }

            // Gọi hàm Vote để thêm đánh giá cho từng sách
            Vote(b1, ("An", 4, "Dễ hiểu"), ("Bình", 3, "Hơi cơ bản"));
            Vote(b2, ("Cường", 5, "Nhiều ví dụ"), ("Dung", 4, "Tốt"), ("An", 5, "Rất hay"), ("Hà", 4, "Ổn"));
            Vote(b3, ("Mai", 5, "Rất chi tiết, nhiều ví dụ thực tế"), ("Hùng", 5, "Phần truy vấn rất hay"),
                    ("Lan", 4, "Cần thêm bài tập"));
            Vote(b4, ("Nam", 3, "Khó"));
            Vote(b5, ("Oanh", 4, "Hợp lý"), ("Phúc", 4, "Đáng đọc"));

            // 7. LƯU TẤT CẢ VÀO CSDL
            // AddRange: Thêm tất cả 6 cuốn sách vào DbSet (chỉ lưu trong bộ nhớ).
            // Lúc này, EF Core sẽ tự động nhận diện các đối tượng con (Review, BookAuthor, PriceOffer) đi kèm.
            db.Books.AddRange(b1, b2, b3, b4, b5, b6);

            // SaveChangesAsync: Thực sự sinh ra các câu lệnh INSERT và gửi xuống SQL Server.
            // EF Core sẽ tự động chèn Author, Book, Review, PriceOffer, BookAuthor theo đúng thứ tự khóa ngoại.
            await db.SaveChangesAsync(ct);

        }
    }
}