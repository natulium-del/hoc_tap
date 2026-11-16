using Microsoft.AspNetCore.Mvc;

namespace QuanLySinhVien.Services
{
    public interface ISinhVienService 
    {
        Task<List<SinhVienDto>> GetAllAsync();
        Task<SinhVien?> FindAsync(int id);
        Task<SinhVien?> FindWithKhoaAsync(int id);
        Task<int> ThemAsync(SinhVien sv);
        Task<bool> CapNhatAsync(SinhVien sv);
        Task<bool> XoaAsync(int id);
        Task<List<Khoa>> GetKhoasAsync();
    }
}
