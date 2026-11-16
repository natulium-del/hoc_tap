using QuanLySinhVien.Models;
using QuanLySinhVien.ViewModels;

namespace QuanLySinhVien.Services
{
    public interface ISinhVienService
    {
        // 1. Truy vấn đọc
        Task<List<SinhVienDto>> GetAllAsync();
        Task<SinhVien?> FindAsync(int id);
        Task<SinhVien?> FindWithKhoaAsync(int id);

        // 2. Thêm / Cập nhật / Xóa mềm
        Task<int> ThemAsync(SinhVien sv);
        Task<bool> CapNhatAsync(SinhVien sv);
        Task<bool> XoaAsync(int id);

        // 3. Dropdown khoa
        Task<List<Khoa>> GetKhoasAsync();

        // 4. Tìm kiếm + Phân trang (Bổ sung)
        Task<PagedResult<SinhVienDto>> TimKiemAsync(
            string? tuKhoa, string? sapXep, int trang = 1, int kichThuoc = 5);

        // 5. Demo N+1 vs Include vs Projection (Bổ sung)
        Task<List<string>> BaoCaoN1Async();
        Task<List<string>> BaoCaoIncludeAsync();
        Task<List<string>> BaoCaoProjectionAsync();

        // 6. Xóa mềm: Danh sách / Khôi phục (Bổ sung)
        Task<List<SinhVien>> GetDaXoaAsync();
        Task<bool> KhoiPhucAsync(int id);

        // 7. Cập nhật / Xóa hàng loạt (Bổ sung)
        Task<int> CongDiemChoLopAsync(string lop, double mucCong);
        Task<int> DonDepAsync();
    }
}