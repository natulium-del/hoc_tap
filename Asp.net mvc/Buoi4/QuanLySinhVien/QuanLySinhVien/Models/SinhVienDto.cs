using Microsoft.AspNetCore.Mvc;

namespace QuanLySinhVien.Models
{
    public record SinhVienDto(int Id, string HoTen, string Lop,
double DiemTrungBinh, string TenKhoa);
}
