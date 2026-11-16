using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class DangKyHoc
    {
        public int SinhVienId { get; set; }
        public SinhVien SinhVien { get; set; } = null!;
        public int MonHocId { get; set; }
        public MonHoc MonHoc { get; set; } = null!;
        public DateOnly NgayDangKy { get; set; }
        [Range(0, 10)]
        public double? Diem { get; set; }
    }
}
