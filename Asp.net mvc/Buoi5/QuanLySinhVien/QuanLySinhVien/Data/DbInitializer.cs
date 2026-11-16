using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Data;

public static class DbInitializer
{
    public static void Seed(AppDbContext db)
    {
        // 1. Áp dụng các migration còn thiếu (tương đương dotnet ef database update)
        db.Database.Migrate();

        // 2. Đã có dữ liệu thì dừng - tránh thêm trùng khi chạy lại ứng dụng
        if (db.Khoas.Any()) return;

        // 3. Danh mục khoa
        var cntt = new Khoa { MaKhoa = "CNTT", TenKhoa = "Công nghệ thông tin" };
        var kt = new Khoa { MaKhoa = "KT", TenKhoa = "Kinh tế" };
        var nna = new Khoa { MaKhoa = "NNA", TenKhoa = "Ngôn ngữ Anh" };

        // 4. Sinh viên - gán navigation Khoa, EF tự điền KhoaId khi lưu
        var ds = new List<SinhVien>
        {
            new() { HoTen = "Nguyễn Văn An", Lop = "CNTT-K18", Email = "an@sv.edu.vn",
                    NgaySinh = new DateOnly(2005, 3, 15), DiemTrungBinh = 8.2, Khoa = cntt },
            new() { HoTen = "Trần Văn Bình", Lop = "CNTT-K18", Email = "binh@sv.edu.vn",
                    NgaySinh = new DateOnly(2005, 7, 2), DiemTrungBinh = 7.8, Khoa = cntt },
            new() { HoTen = "Lê Thị Cúc", Lop = "CNTT-K19", Email = "cuc@sv.edu.vn",
                    NgaySinh = new DateOnly(2006, 1, 20), DiemTrungBinh = 8.5, Khoa = cntt },
            new() { HoTen = "Phạm Văn Dũng", Lop = "KT-K18", Email = "dung@sv.edu.vn",
                    NgaySinh = new DateOnly(2005, 11, 9), DiemTrungBinh = 6.9, Khoa = kt },
            new() { HoTen = "Hoàng Thị Em", Lop = "KT-K19", Email = "em@sv.edu.vn",
                    NgaySinh = new DateOnly(2006, 5, 30), DiemTrungBinh = 9.1, Khoa = kt },
            new() { HoTen = "Vũ Minh Phúc", Lop = "NNA-K18", Email = "phuc@sv.edu.vn",
                    NgaySinh = new DateOnly(2005, 2, 14), DiemTrungBinh = 7.4, Khoa = nna },
            new() { HoTen = "Đặng Thu Giang", Lop = "CNTT-K19", Email = "giang@sv.edu.vn",
                    NgaySinh = new DateOnly(2006, 9, 8), DiemTrungBinh = 5.8, Khoa = cntt },
            new() { HoTen = "Bùi Quốc Huy", Lop = "NNA-K19", Email = "huy@sv.edu.vn",
                    NgaySinh = new DateOnly(2006, 12, 1), DiemTrungBinh = 6.5, Khoa = nna },
        };

        // 5. Đăng ký học - MonHoc Id 1..4 đã có sẵn nhờ HasData ở Bước 4
        var ngay = new DateOnly(2026, 9, 1);
        var dangKy = new List<DangKyHoc>
        {
            new() { SinhVien = ds[0], MonHocId = 1, NgayDangKy = ngay, Diem = 8.0 },
            new() { SinhVien = ds[0], MonHocId = 2, NgayDangKy = ngay, Diem = 7.5 },
            new() { SinhVien = ds[0], MonHocId = 3, NgayDangKy = ngay }, // chưa có điểm
            new() { SinhVien = ds[1], MonHocId = 3, NgayDangKy = ngay },
            new() { SinhVien = ds[2], MonHocId = 4, NgayDangKy = ngay, Diem = 9.0 },
            new() { SinhVien = ds[4], MonHocId = 2, NgayDangKy = ngay, Diem = 8.5 },
        };

        db.Khoas.AddRange(cntt, kt, nna);
        db.SinhViens.AddRange(ds);
        db.DangKyHocs.AddRange(dangKy);

        // 6. Một lần SaveChanges - EF tự sắp thứ tự INSERT: Khoa -> SinhVien -> DangKyHoc
        db.SaveChanges();
    }
}