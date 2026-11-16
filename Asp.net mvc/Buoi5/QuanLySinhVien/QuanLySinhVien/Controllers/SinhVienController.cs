using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLySinhVien.Models;
using QuanLySinhVien.Services;

namespace QuanLySinhVien.Controllers;

public class SinhVienController(ISinhVienService service, ILogger<SinhVienController> logger) : Controller
{
    // Danh sách trường được phép bind - chống tấn công overposting
    private const string TruongThem = "HoTen,Lop,NgaySinh,Email,SoDienThoai,DiemTrungBinh,KhoaId";
    private const string TruongSua = "Id," + TruongThem;

    // GET: /SinhVien?tuKhoa=...&sapXep=...&trang=2
    public async Task<IActionResult> Index(string? tuKhoa, string? sapXep, int trang = 1)
    {
        ViewBag.TuKhoa = tuKhoa;
        ViewBag.SapXep = sapXep;
        var ketQua = await service.TimKiemAsync(tuKhoa, sapXep, trang);
        return View(ketQua);
    }

    // GET: /SinhVien/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var sv = await service.LayChiTietAsync(id);
        return sv is null ? NotFound() : View(sv);
    }

    // GET: /SinhVien/Create
    public async Task<IActionResult> Create()
    {
        await NapDanhSachKhoaAsync();
        return View(new SinhVien { NgaySinh = new DateOnly(2006, 1, 1) });
    }

    // POST: /SinhVien/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(TruongThem)] SinhVien sv)
    {
        if (await service.EmailDaTonTaiAsync(sv.Email))
            ModelState.AddModelError(nameof(SinhVien.Email), "Email này đã được sử dụng");

        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoaAsync(sv.KhoaId); // nạp lại dropdown trước khi trả form
            return View(sv);
        }

        await service.ThemAsync(sv);
        TempData["ThongBao"] = $"Đã thêm sinh viên {sv.HoTen}.";
        return RedirectToAction(nameof(Index)); // POST - Redirect - GET
    }

    // GET: /SinhVien/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var sv = await service.LayDeSuaAsync(id);
        if (sv is null) return NotFound();

        await NapDanhSachKhoaAsync(sv.KhoaId);
        return View(sv);
    }

    // POST: /SinhVien/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind(TruongSua)] SinhVien sv)
    {
        if (id != sv.Id) return BadRequest();

        if (await service.EmailDaTonTaiAsync(sv.Email, sv.Id))
            ModelState.AddModelError(nameof(SinhVien.Email), "Email này đã được sử dụng");

        if (!ModelState.IsValid)
        {
            await NapDanhSachKhoaAsync(sv.KhoaId);
            return View(sv);
        }

        if (!await service.CapNhatAsync(sv)) return NotFound();

        TempData["ThongBao"] = "Đã cập nhật thông tin sinh viên.";
        return RedirectToAction(nameof(Details), new { id = sv.Id });
    }

    // GET: /SinhVien/Delete/5 - trang xác nhận
    public async Task<IActionResult> Delete(int id)
    {
        var sv = await service.LayChiTietAsync(id);
        return sv is null ? NotFound() : View(sv);
    }

    // POST: /SinhVien/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        TempData["ThongBao"] = await service.XoaAsync(id)
            ? "Đã xóa sinh viên." : "Không tìm thấy sinh viên cần xóa.";

        return RedirectToAction(nameof(Index));
    }

    // Nạp danh sách khoa cho thẻ <select>
    private async Task NapDanhSachKhoaAsync(int? khoaDaChon = null)
    {
        var khoas = await service.LayDanhSachKhoaAsync();
        ViewBag.KhoaList = new SelectList(khoas, "Id", "TenKhoa", khoaDaChon);
    }
    public async Task<IActionResult> ThiNghiem()
    {
        logger.LogWarning("===== PHIEN BAN N+1 =====");
        await service.BaoCaoN1Async();

        logger.LogWarning("===== PHIEN BAN INCLUDE =====");
        await service.BaoCaoIncludeAsync();

        logger.LogWarning("===== PHIEN BAN PROJECTION =====");
        var ketQua = await service.BaoCaoProjectionAsync();

        return Content(string.Join(Environment.NewLine, ketQua), "text/plain; charset=utf-8");
    }
    // GET: /SinhVien/ThungRac
    public async Task<IActionResult> ThungRac() => View(await service.LayDaXoaAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> KhoiPhuc(int id)
    {
        TempData["ThongBao"] = await service.KhoiPhucAsync(id)
            ? "Đã khôi phục sinh viên."
            : "Không tìm thấy bản ghi cần khôi phục.";
        return RedirectToAction(nameof(ThungRac));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DonThungRac()
    {
        int soDong = await service.DonThungRacAsync();
        TempData["ThongBao"] = $"Đã xóa vĩnh viễn {soDong} sinh viên.";
        return RedirectToAction(nameof(ThungRac));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CongDiem(string lop, double mucCong)
    {
        // Validate dữ liệu đầu vào
        if (string.IsNullOrWhiteSpace(lop) || mucCong <= 0 || mucCong > 2)
        {
            TempData["ThongBao"] = "Dữ liệu không hợp lệ (mức cộng lớn hơn 0 và không quá 2).";
            return RedirectToAction(nameof(Index));
        }

        // Gọi service xử lý
        int soDong = await service.CongDiemChoLopAsync(lop.Trim(), mucCong);
        TempData["ThongBao"] = $"Đã cộng {mucCong} điểm cho {soDong} sinh viên lớp {lop}.";
        return RedirectToAction(nameof(Index));
    }
}