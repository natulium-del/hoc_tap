using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.ViewModels;

public class DiaChiVm
{
    [Display(Name = "Số nhà")]
    [MaxLength(50)]
    public string? SoNha { get; set; }

    [Display(Name = "Phường/Xã")]
    [MaxLength(50)]
    public string? Phuong { get; set; }

    [Display(Name = "Quận/Huyện")]
    [MaxLength(50)]
    public string? QuanHuyen { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành phố")]
    [Display(Name = "Tỉnh/Thành phố")]
    [MaxLength(50)]
    public string TinhThanh { get; set; } = string.Empty;
}