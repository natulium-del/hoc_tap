using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using QuanLySinhVien.Validations;
using Microsoft.AspNetCore.Mvc;
namespace QuanLySinhVien.ViewModels;

// Bổ sung interface IValidatableObject
public class SinhVienCreateVm : IValidatableObject
{
    // Thuộc tính Id (dùng cho Remote Validation lúc Sửa)
    public int Id { get; set; }

    // CHỈ GIỮ LẠI 1 BỘ ATTRIBUTE DUY NHẤT NÀY:
    [Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
    [RegularExpression(@"^SV\d{7}$", ErrorMessage = "Mã sinh viên phải có dạng SV + 7 chữ số, ví dụ SV2024001")]
    [Remote(action: "KiemTraMaSV", controller: "SinhVien",
        AdditionalFields = nameof(Id),
        ErrorMessage = "Mã sinh viên này đã được sử dụng")]
    [Display(Name = "Mã sinh viên")]
    public string MaSV { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "{0} phải từ {2} đến {1} ký tự")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập lớp")]
    [MaxLength(20, ErrorMessage = "{0} tối đa {1} ký tự")]
    [Display(Name = "Lớp")]
    public string Lop { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateOnly NgaySinh { get; set; } = new DateOnly(2005, 1, 1);

    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [MaxLength(150)]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số, bắt đầu bằng 0")]
    [Display(Name = "Số điện thoại")]
    public string? SoDienThoai { get; set; }

    [Range(0, 10, ErrorMessage = "{0} phải trong khoảng {1} đến {2}")]
    [Display(Name = "Điểm trung bình")]
    public double DiemTrungBinh { get; set; }

    [AllowedValues("Nam", "Nữ", "Khác", ErrorMessage = "Giới tính chỉ nhận Nam, Nữ hoặc Khác")]
    [Display(Name = "Giới tính")]
    public string GioiTinh { get; set; } = "Nam";
    public DiaChiVm DiaChi { get; set; } = new();
    [BindRequired]
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn khoa")]
    [Display(Name = "Khoa")]
    public int KhoaId { get; set; }

    [BindRequired]
    public List<SelectListItem> DanhSachKhoa { get; set; } = [];

    [Display(Name = "Ảnh đại diện")]
    [FileAnh(2)]
    public IFormFile? AnhDaiDien { get; set; }

    // ==========================================
    // PHẦN BỔ SUNG: VALIDATION CHÉO (IValidatableObject)
    // ==========================================
    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        // Luật 1: điểm cao thì bắt buộc có email
        if (DiemTrungBinh >= 9.0 && string.IsNullOrWhiteSpace(Email))
        {
            yield return new ValidationResult(
                "Sinh viên có điểm từ 9.0 trở lên bắt buộc phải khai báo email",
                [nameof(Email), nameof(DiemTrungBinh)]); // gắn cho CẢ HAI trường
        }

        // Luật 2: sinh viên chưa đủ 18 tuổi thì phải có số điện thoại người giám hộ
        var homNay = DateOnly.FromDateTime(DateTime.Today);
        int tuoi = homNay.Year - NgaySinh.Year;

        if (NgaySinh > homNay.AddYears(-tuoi)) tuoi--; // Trừ 1 nếu chưa tới sinh nhật

        if (tuoi < 18 && string.IsNullOrWhiteSpace(SoDienThoai))
        {
            yield return new ValidationResult(
                "Sinh viên dưới 18 tuổi phải khai báo số điện thoại liên hệ",
                [nameof(SoDienThoai)]);
        }

        // Luật 3: có thể lấy dịch vụ từ DI container nếu cần
        // var service = ctx.GetService<ISinhVienService>();
    }
}