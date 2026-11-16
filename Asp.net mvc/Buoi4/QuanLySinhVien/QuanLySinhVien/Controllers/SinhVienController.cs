using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySinhVien.Models;
using QuanLySinhVien.Services;

namespace QuanLySinhVien.Controllers;

public class SinhVienController(ISinhVienService service) : Controller
{
    public async Task<IActionResult> Index()
        => View(await service.GetAllAsync());

    public async Task<IActionResult> Detail(int id)
    {
        var sv = await service.FindWithKhoaAsync(id);
        return sv is null ? NotFound() : View(sv);
    }

    [HttpGet]
    public async Task<IActionResult> Them()
    {
        await NapDanhSachKhoa();
        return View(new SinhVien { NgaySinh = new DateOnly(2005, 1, 1) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(SinhVien sv)
    {
        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoa();
            return View(sv);
        }

        await service.ThemAsync(sv);
        TempData["ThongBao"] = $"Đã thêm sinh viên {sv.HoTen} thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Sua(int id)
    {
        var sv = await service.FindAsync(id);
        if (sv is null) return NotFound();

        await NapDanhSachKhoa(sv.KhoaId);
        return View(sv);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(SinhVien sv)
    {
        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoa(sv.KhoaId);
            return View(sv);
        }

        if (!await service.CapNhatAsync(sv)) return NotFound();
        TempData["ThongBao"] = "Cập nhật thành công!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id)
    {
        TempData["ThongBao"] = await service.XoaAsync(id)
            ? "Đã xóa sinh viên."
            : "Không tìm thấy sinh viên cần xóa.";
        return RedirectToAction(nameof(Index));
    }

    private async Task NapDanhSachKhoa(int? daChon = null)
    {
        var khoas = await service.GetKhoasAsync();
        ViewBag.KhoaList = new SelectList(khoas, "Id", "TenKhoa", daChon);
    }
    public async Task<IActionResult> Index(string? tuKhoa, string? sapXep, int trang = 1)
    {
        ViewBag.TuKhoa = tuKhoa;
        ViewBag.SapXep = sapXep;
        return View(await service.TimKiemAsync(tuKhoa, sapXep, trang));
    }
    public async Task<IActionResult> ThiNghiem()
    {
        Console.WriteLine("===== BAT DAU: PHIEN BAN N+1 =====");
        await service.BaoCaoN1Async();

        Console.WriteLine("===== BAT DAU: PHIEN BAN INCLUDE =====");
        await service.BaoCaoIncludeAsync();

        Console.WriteLine("===== BAT DAU: PHIEN BAN PROJECTION =====");
        var kq = await service.BaoCaoProjectionAsync();

        return Content(string.Join("\n", kq), "text/plain; charset=utf-8");
    }
    public async Task<IActionResult> ThungRac()
    => View(await service.GetDaXoaAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhoiPhuc(int id)
    {
        TempData["ThongBao"] = await service.KhoiPhucAsync(id)
            ? "Đã khôi phục sinh viên."
            : "Không tìm thấy bản ghi.";
        return RedirectToAction(nameof(ThungRac));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CongDiem(string lop, double mucCong)
    {
        int soDong = await service.CongDiemChoLopAsync(lop, mucCong);
        TempData["ThongBao"] = $"Đã cập nhật điểm cho {soDong} sinh viên.";
        return RedirectToAction(nameof(Index));
    }
}