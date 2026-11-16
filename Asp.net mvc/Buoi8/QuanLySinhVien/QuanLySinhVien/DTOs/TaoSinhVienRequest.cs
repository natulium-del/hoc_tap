using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.DTOs;

public record TaoSinhVienRequest(
    [property: Required(ErrorMessage = "Vui lòng nhập mã sinh viên")]
    [property: RegularExpression(@"^SV\d{7}$")] string MaSV,

    [property: Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [property: StringLength(100, MinimumLength = 2)] string HoTen,

    [property: Range(0, 10)] double DiemTrungBinh,

    [property: Range(1, int.MaxValue)] int KhoaId
);