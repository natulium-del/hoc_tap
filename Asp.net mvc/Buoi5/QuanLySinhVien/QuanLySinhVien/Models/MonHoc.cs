using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class MonHoc
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên môn không được để trống")]
        [StringLength(100, ErrorMessage = "Tên môn không được vượt quá 100 ký tự")]
        [Display(Name = "Tên môn")]
        public string TenMon { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Số tín chỉ phải từ 1 đến 10")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; }

        public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
    }
}
