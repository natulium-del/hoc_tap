// ===== Services/SinhVienService.cs =====
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;
using QuanLySinhVien.Models;

namespace QuanLySinhVien.Services;

public class SinhVienService(AppDbContext db) : ISinhVienService
{
    // Danh sách chỉ để hiển thị -> AsNoTracking + projection sang DTO
    public async Task<List<SinhVienDto>> GetAllAsync()
        => await db.SinhViens
            .AsNoTracking()
            .OrderBy(x => x.HoTen)
            .Select(x => new SinhVienDto(
                x.Id, x.HoTen, x.Lop, x.DiemTrungBinh,
                x.Khoa!.TenKhoa))
            .ToListAsync();

    // Dùng khi CẦN SỬA -> KHÔNG dùng AsNoTracking
    public Task<SinhVien?> FindAsync(int id)
        => db.SinhViens.FirstOrDefaultAsync(x => x.Id == id);

    // Dùng khi chỉ hiển thị chi tiết -> nạp sẵn Khoa bằng Include
    public Task<SinhVien?> FindWithKhoaAsync(int id)
        => db.SinhViens.AsNoTracking()
            .Include(x => x.Khoa)
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task<int> ThemAsync(SinhVien sv)
    {
        db.SinhViens.Add(sv);
        await db.SaveChangesAsync();
        return sv.Id;   // Id được CSDL gán tự động
    }

    public async Task<bool> CapNhatAsync(SinhVien sv)
    {
        var cu = await db.SinhViens.FindAsync(sv.Id);
        if (cu is null) return false;

        cu.HoTen = sv.HoTen;            // Change Tracker tự đánh dấu Modified
        cu.Lop = sv.Lop;
        cu.NgaySinh = sv.NgaySinh;
        cu.Email = sv.Email;
        cu.DiemTrungBinh = sv.DiemTrungBinh;
        cu.KhoaId = sv.KhoaId;

        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> XoaAsync(int id)
    {
        var sv = await db.SinhViens.FindAsync(id);
        if (sv is null) return false;

        db.SinhViens.Remove(sv);
        await db.SaveChangesAsync();
        return true;
    }

    public Task<List<Khoa>> GetKhoasAsync()
        => db.Khoas.AsNoTracking().OrderBy(x => x.TenKhoa).ToListAsync()

        public async Task<PagedResult<SinhVienDto>> TimKiemAsync(
    string? tuKhoa, string? sapXep, int trang = 1, int kichThuoc = 5)
    {
        // 1. Bắt đầu từ IQueryable - chưa gửi SQL nào
        var q = db.SinhViens.AsNoTracking();

        // 2. Lọc động theo từ khóa
        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            tuKhoa = tuKhoa.Trim();
            q = q.Where(x => x.HoTen.Contains(tuKhoa) || x.Lop.Contains(tuKhoa));
        }

        // 3. Sắp xếp - BẮT BUỘC có trước Skip/Take
        q = sapXep switch
        {
            "diem_desc" => q.OrderByDescending(x => x.DiemTrungBinh),
            "diem_asc" => q.OrderBy(x => x.DiemTrungBinh),
            "ten_desc" => q.OrderByDescending(x => x.HoTen),
            _ => q.OrderBy(x => x.HoTen)
        };

        // 4. Đếm tổng số TRƯỚC khi phân trang (truy vấn COUNT riêng)
        int tongSo = await q.CountAsync();

        // 5. Phân trang trên CSDL và chiếu sang DTO
        var items = await q
            .Skip((trang - 1) * kichThuoc)
            .Take(kichThuoc)
            .Select(x => new SinhVienDto(x.Id, x.HoTen, x.Lop,
                x.DiemTrungBinh, x.Khoa!.TenKhoa))
            .ToListAsync();

        return new PagedResult<SinhVienDto>
        {
            Items = items,
            TongSo = tongSo,
            Trang = trang,
            KichThuoc = kichThuoc
        };
        // PHIÊN BẢN SAI - gây lỗi N + 1
public async Task<List<string>> BaoCaoN1Async()
    {
        var ketQua = new List<string>();
        var dsSv = await db.SinhViens.AsNoTracking().ToListAsync();   // 1 truy vấn

        foreach (var sv in dsSv)
        {
            // Mỗi vòng lặp là MỘT truy vấn xuống CSDL -> N truy vấn
            var khoa = await db.Khoas.FindAsync(sv.KhoaId);
            ketQua.Add($"{sv.HoTen} - {khoa?.TenKhoa}");
        }

        return ketQua;   // tổng cộng: 1 + N câu lệnh SQL
    }

    // PHIÊN BẢN ĐÚNG 1 - dùng Include (một truy vấn có JOIN)
    public async Task<List<string>> BaoCaoIncludeAsync()
        => (await db.SinhViens.AsNoTracking()
            .Include(x => x.Khoa)
            .ToListAsync())
            .Select(sv => $"{sv.HoTen} - {sv.Khoa?.TenKhoa}")
            .ToList();   // 1 câu lệnh SQL

    // PHIÊN BẢN ĐÚNG 2 - projection, nhanh nhất vì chỉ lấy 2 cột
    public Task<List<string>> BaoCaoProjectionAsync()
        => db.SinhViens.AsNoTracking()
            .Select(sv => sv.HoTen + " - " + sv.Khoa!.TenKhoa)
            .ToListAsync();   // 1 câu lệnh SQL, ít cột nhất
                              // Xóa mềm - chỉ đánh dấu, không xóa thật khỏi CSDL
    public async Task<bool> XoaAsync(int id)
    {
        var sv = await db.SinhViens.FindAsync(id);
        if (sv is null) return false;

        sv.DaXoa = true;              // Change Tracker đánh dấu Modified
        await db.SaveChangesAsync();   // sinh UPDATE chứ không phải DELETE
        return true;
    }

    // Danh sách bản ghi đã xóa mềm - phải TẮT bộ lọc mới thấy
    public Task<List<SinhVien>> GetDaXoaAsync()
        => db.SinhViens.IgnoreQueryFilters()
            .Where(x => x.DaXoa)
            .AsNoTracking()
            .ToListAsync();

    // Khôi phục bản ghi đã xóa mềm
    public async Task<bool> KhoiPhucAsync(int id)
    {
        var sv = await db.SinhViens.IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (sv is null) return false;

        sv.DaXoa = false;
        await db.SaveChangesAsync();
        return true;
    }

    // Cộng điểm hàng loạt - MỘT câu UPDATE duy nhất, không tải dữ liệu về
    public Task<int> CongDiemChoLopAsync(string lop, double mucCong)
        => db.SinhViens
            .Where(x => x.Lop == lop)
            .ExecuteUpdateAsync(s => s.SetProperty(
                x => x.DiemTrungBinh,
                x => x.DiemTrungBinh + mucCong));

    // Xóa vĩnh viễn hàng loạt những bản ghi đã xóa mềm quá lâu
    public Task<int> DonDepAsync()
        => db.SinhViens.IgnoreQueryFilters()
            .Where(x => x.DaXoa)
            .ExecuteDeleteAsync();
}
}