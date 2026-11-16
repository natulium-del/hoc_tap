using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class SinhVien
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string MaSV { get; set; } = string.Empty;
        [Required(ErrorMessage = "Họ tên là bắt buộc không được trùng")]
        [MaxLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        public string HoTen { get; set; } = string.Empty;
        [Required(ErrorMessage = "Lớp là bắt buộc")]
        [MaxLength(20, ErrorMessage = "Tên lớp không được vượt quá 20 ký tự")]
        public string Lop { get; set; } = string.Empty;
        [Range(0, 10, ErrorMessage = "Điểm trung bình phải nằm trong khoảng từ 0 đến 10")]
        public double DiemTrungBinh { get; set; }
        [DataType(DataType.Date)]
        public DateOnly NgaySinh { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ"), MaxLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string? Email { get; set; } 
        public bool DaXoa { get; set; } = false;
        [MaxLength(15)]
        public string? SoDienThoai { get; set; }
        //Quan hệ N-1 với Khoa 
        public int KhoaId { get; set; }
        public Khoa? Khoa { get; set; }

        //Quan hệ N-N với môn học
        public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
        public string? AnhDaiDien { get; set; }
        public bool LaQuanTri { get; set; } = false;

    }
}
