using System.ComponentModel.DataAnnotations;

namespace QuanLySinhVien.Validations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class TuoiToiThieuAttribute(int tuoiToiThieu) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        // null để cho [Required] xử lý - nguyên tắc mỗi attribute làm một việc
        if (value is null) return ValidationResult.Success;

        DateOnly ngaySinh = value switch
        {
            DateOnly d => d,
            DateTime dt => DateOnly.FromDateTime(dt),
            _ => throw new InvalidOperationException(
                "TuoiToiThieu chỉ dùng cho DateOnly hoặc DateTime")
        };

        var homNay = DateOnly.FromDateTime(DateTime.Today);

        if (ngaySinh > homNay)
        {
            return new ValidationResult("Ngày sinh không được ở tương lai",
                [ctx.MemberName!]);
        }

        int tuoi = homNay.Year - ngaySinh.Year;

        // Xử lý trường hợp chưa tới sinh nhật trong năm nay
        if (ngaySinh > homNay.AddYears(-tuoi))
        {
            tuoi--;
        }

        if (tuoi < tuoiToiThieu)
        {
            return new ValidationResult(
                ErrorMessage ?? $"Phải đủ {tuoiToiThieu} tuổi trở lên (hiện {tuoi} tuổi)",
                [ctx.MemberName!]);
        }

        return ValidationResult.Success;
    }
}