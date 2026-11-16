using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.ViewModels;

public class DongDiemVm
{
    public int SinhVienId { get; set; }

    public string HoTen { get; set; } = string.Empty; // chỉ để hiển thị

    [Range(0, 10, ErrorMessage = "Điểm phải từ {1} đến {2}")]
    [Display(Name = "Điểm")]
    public double? Diem { get; set; }
}

public class NhapDiemVm
{
    public string Lop { get; set; } = string.Empty;

    public List<DongDiemVm> DanhSach { get; set; } = [];
}