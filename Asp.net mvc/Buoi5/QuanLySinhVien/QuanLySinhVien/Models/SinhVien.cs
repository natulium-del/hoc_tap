using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Models
{
    public class SinhVien
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lớp không được để trống")]
        [StringLength(20, ErrorMessage = "Lớp không được vượt quá 20 ký tự")]
        [Display(Name = "Lớp")]
        public string Lop { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày Sinh")]
        public DateOnly NgaySinh { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SoDienThoai { get; set; }

        [Range(0, 10, ErrorMessage = "Điểm trung bình phải nằm trong khoảng từ 0 đến 10")]
        [Display(Name = "Điểm Trung Bình")]
        public double DiemTrungBinh { get; set; }

        public bool DaXoa { get; set; }

        //Quan hệ 1 - N với khoa
        [Required(ErrorMessage = "Khoa không được để trống")]
        [Display(Name = "Khoa")]
        public int KhoaId { get; set; }
        public Khoa? Khoa { get; set; }

        //Quan hệ N-N với môn học thông qua bảng kết nối DangKyHoc
        public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

    }

}
