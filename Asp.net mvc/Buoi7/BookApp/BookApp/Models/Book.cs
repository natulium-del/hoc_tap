using Microsoft.EntityFrameworkCore; // Thư viện chứa các Data Annotation của EF Core (như [Precision])
using System.ComponentModel.DataAnnotations; // Thư viện chứa các Data Annotation kiểm tra dữ liệu (như [Required], [StringLength])

namespace BookApp.Models // Khai báo không gian tên (thư mục) chứa các Entity
{
    // Lớp Book đại diện cho bảng Books trong cơ sở dữ liệu
    public class Book
    {
        // Khóa chính (Primary Key) của bảng. EF Core tự hiểu "Id" là khóa chính tăng tự động.
        public int Id { get; set; }

        // [Required]: Bắt buộc phải có giá trị (tương đương NOT NULL trong SQL)
        // [StringLength(200)]: Độ dài tối đa là 200 ký tự (tương đương NVARCHAR(200))
        // Gán mặc định là "" để tránh cảnh báo lỗi Nullable
        [Required, StringLength(200)]
        public string Title { get; set; } = "";

        // Dấu ? nghĩa là thuộc tính này được phép null (có thể không có mô tả)
        [StringLength(200)]
        public string? Description { get; set; }

        // DateOnly: Chỉ lưu ngày (không lưu giờ phút giây). Tương đương kiểu DATE trong SQL.
        public DateOnly PublishedOn { get; set; }

        // [Precision(9, 2)]: Cấu hình cho kiểu decimal trong SQL.
        // Tổng cộng 9 chữ số, trong đó có 2 chữ số ở phần thập phân (Ví dụ: 1234567.89)
        [Precision(9, 2)]
        public decimal Price { get; set; }

        // Đường dẫn ảnh (có thể null nếu sách không có ảnh)
        [StringLength(300)]
        public string? ImageUrl { get; set; }

        // Cờ đánh dấu xóa mềm. false = chưa xóa, true = đã xóa (bị ẩn đi nhờ Global Query Filter)
        public bool SoftDeleted { get; set; }

        // ==========================================
        // PHẦN QUAN HỆ (NAVIGATION PROPERTIES)
        // Các thuộc tính này giúp EF Core liên kết các bảng lại với nhau.
        // ==========================================

        // Quan hệ 1 - 1: Một cuốn sách có nhiều nhất 1 khuyến mãi (có thể null nếu không khuyến mãi)
        public PriceOffer? Promotion { get; set; }

        // Quan hệ 1 - N: Một cuốn sách có nhiều đánh giá (Review).
        // Khởi tạo bằng [] (List rỗng) để tránh lỗi NullReferenceException khi thêm dữ liệu.
        public List<Review> Reviews { get; set; } = [];

        // Quan hệ N - N (Skip Navigation): Một sách có nhiều tác giả, một tác giả viết nhiều sách.
        // EF Core tự động hiểu mối quan hệ này thông qua bảng trung gian (mà ta sẽ cấu hình sau).
        public List<Author> Authors { get; set; } = [];

        // Quan hệ N - N (thông qua bảng trung gian): Bảng nối BookAuthor.
        // Dùng bảng này khi ta cần thêm thuộc tính phụ (Payload) như cột "Order" (thứ tự tác giả).
        public List<BookAuthor> BookAuthors { get; set; } = [];
    }
}