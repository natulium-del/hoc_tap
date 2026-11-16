using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class Khoa 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã khoa không được để trống")]
        [StringLength(10)]
        [Display(Name = "Mã Khoa")]
        public string MaKhoa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên khoa không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên Khoa")]
        public string TenKhoa { get; set; } = string.Empty;

        // 1 khoa có nhiều sinh viên
        public ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();

    }
}
