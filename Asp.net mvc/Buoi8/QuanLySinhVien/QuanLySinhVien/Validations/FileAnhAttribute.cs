using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http; // Cần cho IFormFile

namespace QuanLySinhVien.Validations;

public class FileAnhAttribute(int maxMb = 2) : ValidationAttribute
{
    private static readonly string[] DuoiChoPhep = [".jpg", ".jpeg", ".png"];

    protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
    {
        if (value is not IFormFile file || file.Length == 0)
            return ValidationResult.Success; // Không bắt buộc tải ảnh

        var duoi = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!DuoiChoPhep.Contains(duoi))
        {
            return new ValidationResult(
                $"Chỉ chấp nhận các định dạng: {string.Join(", ", DuoiChoPhep)}",
                [ctx.MemberName!]);
        }

        if (file.Length > maxMb * 1024 * 1024)
        {
            return new ValidationResult(
                $"Kích thước tối đa {maxMb} MB",
                [ctx.MemberName!]);
        }

        return ValidationResult.Success;
    }
}