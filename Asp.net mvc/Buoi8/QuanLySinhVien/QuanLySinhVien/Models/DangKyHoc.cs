using Microsoft.AspNetCore.Mvc;

namespace QuanLySinhVien.Models
{
    public class DangKyHoc
    {
        public int SinhVienId { get; set; }              // FK → SinhVien
        public SinhVien SinhVien { get; set; } = null!;  // Navigation
        public int MonHocId { get; set; }                // FK → MonHoc
        public MonHoc MonHoc { get; set; } = null!;      // Navigation
        public DateOnly NgayDangKy { get; set; }         // Ngày đăng ký
        public double? Diem { get; set; }                // Điểm (nullable)
    }
}
