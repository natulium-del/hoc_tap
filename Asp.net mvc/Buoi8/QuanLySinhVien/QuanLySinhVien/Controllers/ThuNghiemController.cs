using Microsoft.AspNetCore.Mvc;

namespace QuanLySinhVien.Controllers;

public class ThuNghiemController : Controller
{
    // /ThuNghiem/Xem/5 hoặc /ThuNghiem/Xem?id=5
    public IActionResult Xem(int id)
        => Content($"id = {id} | ModelState hợp lệ: {ModelState.IsValid} | "
                 + $"số lỗi: {ModelState.ErrorCount}",
                   "text/plain; charset=utf-8");

    // /ThuNghiem/Tim?tuKhoa=an&trang=2
    public IActionResult Tim(string? tuKhoa, int trang = 1)
        => Content($"tuKhoa = [{tuKhoa ?? "null"}] | trang = {trang}",
                   "text/plain; charset=utf-8");

    // /ThuNghiem/Loc?conHoatDong=true&ngay=2026-09-24
    public IActionResult Loc(bool conHoatDong, DateOnly ngay)
        => Content($"conHoatDong = {conHoatDong} | ngay = {ngay:dd/MM/yyyy} | "
                 + $"hợp lệ: {ModelState.IsValid}",
                   "text/plain; charset=utf-8");

    // Thí nghiệm thứ tự ưu tiên: route và query cùng tên id
    // /ThuNghiem/UuTien/10?id=99
    public IActionResult UuTien(int id)
        => Content($"Giá trị nhận được: {id}", "text/plain; charset=utf-8");
}