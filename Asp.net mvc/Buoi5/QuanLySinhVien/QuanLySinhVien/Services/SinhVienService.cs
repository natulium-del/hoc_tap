using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;
using System.Linq;

namespace QuanLySinhVien.Services
{
    public class SinhVienService(AppDbContext db): ISinhVienService
    {
            public async Task<PageResult<SinhVienDto>> TimKiemAsync(string? tuKhoa, string? sapXep, int trang = 1, int kichThuoc = 5)
        {
            if(trang <1) trang = 1;
            // (1) Bắt đầu từ IQueryable - CHƯA gửi câu SQL nào
            IQueryable <SinhVien> q = db.SinhViens.AsNoTracking();

            // (2) Lọc động theo từ khóa
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                tuKhoa = tuKhoa.Trim();
                q = q.Where(x => x.HoTen.Contains(tuKhoa) || x.Lop.Contains(tuKhoa));
            }

            // (3) Sắp xếp - BẮT BUỘC có trước Skip/Take
            q = sapXep switch
            {
                "ten_desc" => q.OrderByDescending(x => x.HoTen),
                "diem_desc" => q.OrderByDescending(x => x.DiemTrungBinh).ThenBy(x => x.HoTen),
                "diem_asc" => q.OrderBy(x => x.DiemTrungBinh).ThenBy(x => x.HoTen),
                _ => q.OrderBy(x => x.HoTen)
            };

            // (4) Đếm tổng số bản ghi - câu SQL COUNT riêng
            int tongSo = await q.CountAsync();

            // (5) Phân trang + projection sang DTO - chỉ lấy đúng kichThuoc dòng
            var items = await q
                .Skip((trang - 1) * kichThuoc)
                .Take(kichThuoc)
                .Select(x => new SinhVienDto(
                    x.Id,
                    x.HoTen,
                    x.Lop,
                    x.DiemTrungBinh,
                    x.Khoa!.TenKhoa))
                .ToListAsync();

            return new PageResult<SinhVienDto>
            {
                Items = items,
                TongSo = tongSo,
                Trang = trang,
                KichThuoc = kichThuoc
            };
        }

        // Chi tiết để HIỂN THỊ: nạp sẵn dữ liệu liên quan (Eager Loading)
        public Task<SinhVien?> LayChiTietAsync(int id) =>
            db.SinhViens.AsNoTracking()
                .Include(x => x.Khoa)
                .Include(x => x.DangKyHocs).ThenInclude(d => d.MonHoc)
                .FirstOrDefaultAsync(x => x.Id == id);

        // Dữ liệu để SỬA: giữ tracking
        public Task<SinhVien?> LayDeSuaAsync(int id) =>
            db.SinhViens.FirstOrDefaultAsync(x => x.Id == id);

        public Task<List<Khoa>> LayDanhSachKhoaAsync() =>
            db.Khoas.AsNoTracking().OrderBy(k => k.TenKhoa).ToListAsync();

        // Kiểm tra email trùng, kể cả sinh viên đã bị xóa mềm (Bước 14)
        public async Task<bool> EmailDaTonTaiAsync(string? email, int boQuaId = 0)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return await db.SinhViens.IgnoreQueryFilters()
                .AnyAsync(x => x.Email == email && x.Id != boQuaId);
        }

        public async Task<int> ThemAsync(SinhVien sv)
        {
            db.SinhViens.Add(sv);            // trạng thái Added
            await db.SaveChangesAsync();     // INSERT; sv.Id được CSDL gán tự động
            return sv.Id;
        }

        public async Task<bool> CapNhatAsync(SinhVien sv)
        {
            var cu = await db.SinhViens.FindAsync(sv.Id);   // trạng thái Unchanged
            if (cu is null) return false;

            cu.HoTen = sv.HoTen;     // Change Tracker tự đánh dấu Modified
            cu.Lop = sv.Lop;
            cu.NgaySinh = sv.NgaySinh;
            cu.Email = sv.Email;
            cu.SoDienThoai = sv.SoDienThoai;
            cu.DiemTrungBinh = sv.DiemTrungBinh;
            cu.KhoaId = sv.KhoaId;

            await db.SaveChangesAsync();     // UPDATE chỉ những cột thực sự thay đổi
            return true;
        }

        public async Task<bool> XoaAsync(int id)
        {
            var sv = await db.SinhViens.FindAsync(id);
            if (sv is null) return false;

            //db.SinhViens.Remove(sv);         // trạng thái Deleted -> DELETE
            sv.DaXoa = true;
            await db.SaveChangesAsync();     // (Bước 14 sẽ đổi thành xóa mềm)
            return true;
        }
        // PHIÊN BẢN SAI - lỗi N + 1
        public async Task<List<string>> BaoCaoN1Async()
        {
            var ketQua = new List<string>();
            var dsSv = await db.SinhViens.AsNoTracking().ToListAsync(); // 1 câu SQL

            foreach (var sv in dsSv)
            {
                // Mỗi vòng lặp gửi thêm 1 câu SQL => N câu
                var khoa = await db.Khoas.AsNoTracking()
                    .FirstAsync(k => k.Id == sv.KhoaId);
                ketQua.Add($"{sv.HoTen} - {khoa.TenKhoa}");
            }

            return ketQua; // tổng: 1 + N câu
        }

        // KHẮC PHỤC 1 - Eager Loading: 1 câu SQL có JOIN
        public async Task<List<string>> BaoCaoIncludeAsync()
        {
            var ds = await db.SinhViens.AsNoTracking()
                .Include(x => x.Khoa)
                .ToListAsync();

            return ds.Select(sv => $"{sv.HoTen} - {sv.Khoa!.TenKhoa}").ToList();
        }

        // KHẮC PHỤC 2 - Projection: 1 câu SQL, chỉ lấy đúng cột cần dùng (nhanh nhất)
        public Task<List<string>> BaoCaoProjectionAsync() =>
            db.SinhViens.AsNoTracking()
                .Select(sv => sv.HoTen + " - " + sv.Khoa!.TenKhoa)
                .ToListAsync();
        // Danh sách đã xóa - phải tắt bộ lọc mới nhìn thấy
        public Task<List<SinhVien>> LayDaXoaAsync() =>
            db.SinhViens.IgnoreQueryFilters()
                .AsNoTracking()
                .Where(x => x.DaXoa)
                .OrderBy(x => x.HoTen)
                .ToListAsync();

        public async Task<bool> KhoiPhucAsync(int id)
        {
            var sv = await db.SinhViens.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == id && x.DaXoa);
            if (sv is null) return false;

            sv.DaXoa = false;
            await db.SaveChangesAsync();
            return true;
        }

        // Cộng điểm cả lớp - MỘT câu UPDATE, không tải dữ liệu về bộ nhớ, tối đa 10 điểm
        public Task<int> CongDiemChoLopAsync(string lop, double mucCong) =>
            db.SinhViens
                .Where(x => x.Lop == lop)
                .ExecuteUpdateAsync(s => s.SetProperty(
                    x => x.DiemTrungBinh,
                    x => x.DiemTrungBinh + mucCong > 10 ? 10 : x.DiemTrungBinh + mucCong));

        // Xóa vĩnh viễn toàn bộ bản ghi trong thùng rác - MỘT câu DELETE
        public Task<int> DonThungRacAsync() =>
            db.SinhViens.IgnoreQueryFilters()
                .Where(x => x.DaXoa)
                .ExecuteDeleteAsync(); // DangKyHoc liên quan bị xóa theo nhờ ON DELETE CASCADE

    }
    }

