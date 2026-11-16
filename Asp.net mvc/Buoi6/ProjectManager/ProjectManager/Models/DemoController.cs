using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManager.Data;
using ProjectManager.Models;

namespace ProjectManager.Controllers;

public class DemoController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var vm = new DemoViewModel();

        // ① 1 – N: đếm và cộng qua collection navigation – dịch thành truy vấn con trong SQL
        vm.Customers = await db.Customers
            .OrderBy(c => c.Name)
            .Select(c => new CustomerSummary(c.Name, c.Projects.Count, c.Projects.Sum(p => p.Budget)))
            .ToListAsync();

        // ② Kế thừa: truy vấn lớp cơ sở, EF Core tạo đúng đối tượng Developer / Tester
        var resources = await db.Resources.AsNoTracking().OrderBy(r => r.Id).ToListAsync();
        vm.Resources = resources.Select(r => r switch
        {
            Developer d => new ResourceRow(d.FullName, "Developer", $"{d.MainLanguage} – {d.YearsOfExperience} năm"),
            Tester t => new ResourceRow(t.FullName, "Tester", t.ToolName),
            _ => new ResourceRow(r.FullName, "?", "")
        }).ToList();

        // Chỉ lấy lớp con: WHERE [ResourceType] = N'DEV'
        vm.Developers = await db.Developers.Select(d => d.FullName).ToListAsync();
        // Cách tương đương: db.Resources.OfType<Developer>()

        // ③ 1 – 1: LeftJoin (EF Core 10) – giữ cả nhân sự chưa có hồ sơ
        var profiles = await db.Resources
            .LeftJoin(
                db.ResourceProfiles,
                r => r.Id,
                p => p.ResourceId,
                (r, p) => new { r.Id, r.FullName, Phone = p == null ? null : p.Phone })
            .OrderBy(x => x.Id)
            .ToListAsync();
        vm.Profiles = profiles.Select(x => new ProfileRow(x.FullName, x.Phone)).ToList();
        // Cách dùng navigation: db.Resources.Include(r => r.Profile)

        // ④ N – N: Include skip navigation (Tags) và entity nối (Assignments -> Resource)
        var projects = await db.Projects
            .AsNoTracking()
            .Include(p => p.Tags)
            .Include(p => p.Assignments).ThenInclude(a => a.Resource)
            .OrderBy(p => p.Id)
            .ToListAsync();

        vm.Projects = projects.Select(p => new ProjectRow(
            p.Id,
            p.Name,
            string.Join(", ", p.Tags.Select(t => t.Name)),
            string.Join(", ", p.Assignments.Select(a =>
                $"{a.Resource.FullName.Split(' ').Last()} ({a.Role})"))
        )).ToList();

        return View(vm);
    }

    // Thí nghiệm Restrict (Bước 6)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer is null) return NotFound();

        try
        {
            db.Customers.Remove(customer);
            await db.SaveChangesAsync();
            TempData["Ok"] = $"Đã xóa khách hàng {customer.Name}.";
        }
        catch (DbUpdateException ex)
        {
            TempData["Error"] = $"Không xóa được khách hàng {customer.Name}: còn dự án liên kết (Restrict). "
                + ex.InnerException?.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    // Thí nghiệm Cascade (Bước 6)
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteResource(int id)
    {
        var resource = await db.Resources.FindAsync(id);
        if (resource is null) return NotFound();

        db.Resources.Remove(resource);
        await db.SaveChangesAsync(); // CSDL tự xóa ResourceProfiles và Assignments liên quan
        TempData["Ok"] = $"Đã xóa nhân sự {resource.FullName} cùng hồ sơ và phân công.";
        return RedirectToAction(nameof(Index));
    }
}