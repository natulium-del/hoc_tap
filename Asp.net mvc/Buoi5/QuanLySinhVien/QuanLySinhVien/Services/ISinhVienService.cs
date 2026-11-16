using QuanLySinhVien.Models;

namespace QuanLySinhVien.Services
{
    public interface ISinhVienService
    {
        //Đọc dữ liệu
        Task<PageResult<SinhVienDto>> TimKiemAsync(string? tuKhoa, string? sapXep, int trang = 1, int kichThuoc = 5);

        Task<SinhVien?> LayChiTietAsync(int id);   // kèm Khoa và DangKyHoc, chỉ đọc
        Task<SinhVien?> LayDeSuaAsync(int id);     // có tracking
        Task<List<Khoa>> LayDanhSachKhoaAsync();
        Task<bool> EmailDaTonTaiAsync(string? email, int boQuaId = 0);

        // ----- Ghi dữ liệu -----
        Task<int> ThemAsync(SinhVien sv);
        Task<bool> CapNhatAsync(SinhVien sv);
        Task<bool> XoaAsync(int id);

        Task<List<string>> BaoCaoN1Async();

        Task<List<string>> BaoCaoIncludeAsync();

        Task<List<string>> BaoCaoProjectionAsync();

        Task<List<SinhVien>> LayDaXoaAsync();
        Task<bool> KhoiPhucAsync(int id);
        Task<int> CongDiemChoLopAsync(string lop, double mucCong);
        Task<int> DonThungRacAsync();
    }
}
