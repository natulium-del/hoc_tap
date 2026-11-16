using Microsoft.AspNetCore.Mvc;
using QuanLySinhVien.Services;

namespace QuanLySinhVien.Controllers;

[Route("bao-cao")] // tiền tố cho cả controller
public class BaoCaoController : Controller
{
    private readonly ISinhVienService _service;

    public BaoCaoController(ISinhVienService service) => _service = service;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var ds = await _service.GetAllAsync();
        return Content($"Tổng số sinh viên: {ds.Count}");
    }

    [HttpGet("theo-lop/{lop}")]
    public async Task<IActionResult> TheoLop(string lop)
    {
        // Phải await để lấy List<SinhVienDto> trước khi dùng LINQ (.Where)
        var ds = await _service.GetAllAsync();
        var ketQua = ds.Where(x => x.Lop == lop).ToList();
        return Json(ketQua);
    }

    [HttpGet("diem-cao/{nguong:double}")]
    public async Task<IActionResult> DiemCao(double nguong)
    {
        var ds = await _service.GetAllAsync();
        var ketQua = ds.Where(x => x.DiemTrungBinh >= nguong).ToList();
        return Json(ketQua);
    }
}