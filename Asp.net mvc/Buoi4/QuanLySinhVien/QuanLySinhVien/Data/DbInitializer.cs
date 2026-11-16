// ===== Data/DbInitializer.cs =====
using QuanLySinhVien.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLySinhVien.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        // 1. Áp dụng mọi migration còn thiếu (thay cho dotnet ef database update)
        db.Database.Migrate();

        // 2. Nếu đã có dữ liệu thì dừng - tránh tạo trùng khi chạy lại
        if (db.Khoas.Any()) return;

        // 3. Thêm Khoa trước vì SinhVien phụ thuộc khóa ngoại KhoaId
        var cntt = new Khoa { MaKhoa = "CNTT", TenKhoa = "Công nghệ thông tin" };
        var kinhTe = new Khoa { MaKhoa = "KT", TenKhoa = "Kinh tế" };
        var nnAnh = new Khoa { MaKhoa = "NNA", TenKhoa = "Ngôn ngữ Anh" };
        db.Khoas.AddRange(cntt, kinhTe, nnAnh);
        db.SaveChanges(); // lưu để Khoa có Id thật

        // 4. Thêm SinhVien - dùng navigation property, EF tự gán KhoaId
        db.SinhViens.AddRange(
            new SinhVien
            {
                HoTen = "Nguyễn Văn A",
                Lop = "CNTT-K18",
                NgaySinh = new DateOnly(2005, 3, 15),
                Email = "a@sv.edu.vn",
                DiemTrungBinh = 8.2,
                Khoa = cntt
            },
            new SinhVien
            {
                HoTen = "Trần Văn Bình",
                Lop = "CNTT-K18",
                NgaySinh = new DateOnly(2005, 7, 2),
                Email = "binh@sv.edu.vn",
                DiemTrungBinh = 7.8,
                Khoa = cntt
            },
            new SinhVien
            {
                HoTen = "Lê Thị Cúc",
                Lop = "CNTT-K19",
                NgaySinh = new DateOnly(2006, 1, 20),
                Email = "cuc@sv.edu.vn",
                DiemTrungBinh = 8.5,
                Khoa = cntt
            },
            new SinhVien
            {
                HoTen = "Phạm Văn Dũng",
                Lop = "KT-K18",
                NgaySinh = new DateOnly(2005, 11, 9),
                Email = "dung@sv.edu.vn",
                DiemTrungBinh = 6.9,
                Khoa = kinhTe
            },
            new SinhVien
            {
                HoTen = "Hoàng Thị Em",
                Lop = "KT-K19",
                NgaySinh = new DateOnly(2006, 5, 30),
                Email = "em@sv.edu.vn",
                DiemTrungBinh = 9.1,
                Khoa = kinhTe
            },
            new SinhVien
            {
                HoTen = "Vũ Minh Phúc",
                Lop = "NNA-K18",
                NgaySinh = new DateOnly(2005, 2, 14),
                Email = "phuc@sv.edu.vn",
                DiemTrungBinh = 7.4,
                Khoa = nnAnh
            });

        db.SaveChanges();

        var toan = new MonHoc { TenMonHoc = "Toán rời rạc", SoTinChi = 3 };
        var csdl = new MonHoc { TenMonHoc = "Cơ sở dữ liệu", SoTinChi = 4 };
        var web = new MonHoc { TenMonHoc = "Lập trình Web", SoTinChi = 4 };
        db.MonHocs.AddRange(toan, csdl, web);
        db.SaveChanges();

        var dsSv = db.SinhViens.ToList();
        var homNay = DateOnly.FromDateTime(DateTime.Now);

        db.DangKyHocs.AddRange(
            new DangKyHoc { SinhVienId = dsSv[0].Id, MonHocId = web.Id, NgayDangKy = homNay, Diem = 8.0 },
            new DangKyHoc { SinhVienId = dsSv[0].Id, MonHocId = csdl.Id, NgayDangKy = homNay, Diem = 7.5 },
            new DangKyHoc { SinhVienId = dsSv[1].Id, MonHocId = web.Id, NgayDangKy = homNay },
            new DangKyHoc { SinhVienId = dsSv[2].Id, MonHocId = toan.Id, NgayDangKy = homNay, Diem = 9.0 });

        db.SaveChanges();
    }
}