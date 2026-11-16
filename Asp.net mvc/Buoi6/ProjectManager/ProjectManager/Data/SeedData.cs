using Microsoft.EntityFrameworkCore;
using ProjectManager.Models;

namespace ProjectManager.Data;

public static class SeedData
{
    // Chữ ký phù hợp với UseAsyncSeeding (EF Core 9+)
    public static async Task SeedAsync(DbContext context, bool _, CancellationToken ct)
    {
        var db = (AppDbContext)context;
        if (await db.Customers.AnyAsync(ct)) return; // đã có dữ liệu

        // 1 – N
        var fpt = new Customer { Name = "FPT Software", Email = "contact@fpt.vn" };
        var viettel = new Customer { Name = "Viettel Solutions", Email = "info@viettel.vn" };
        var vnpt = new Customer { Name = "VNPT IT", Email = "it@vnpt.vn" }; // chưa có dự án

        // N – N đơn giản
        Tag web = new() { Name = "Web" };
        Tag mobile = new() { Name = "Mobile" };
        Tag ai = new() { Name = "AI" };

        var crm = new Project
        {
            Name = "CRM",
            StartDate = new DateOnly(2026, 1, 5),
            Budget = 500_000_000,
            Customer = fpt,
            Tags = [web]
        };
        var erp = new Project
        {
            Name = "ERP",
            StartDate = new DateOnly(2026, 3, 1),
            Budget = 1_200_000_000,
            Customer = fpt,
            Tags = [web, ai]
        };
        var bank = new Project
        {
            Name = "Mobile Banking",
            StartDate = new DateOnly(2026, 2, 15),
            Budget = 800_000_000,
            Customer = viettel,
            Tags = [mobile]
        };

        // Kế thừa + 1 – 1
        var an = new Developer
        {
            FullName = "Nguyễn Văn An",
            Email = "an@pm.vn",
            MainLanguage = "C#",
            YearsOfExperience = 5,
            Profile = new() { Phone = "0901000001", Bio = "Tech lead .NET" }
        };
        var binh = new Tester
        {
            FullName = "Trần Thị Bình",
            Email = "binh@pm.vn",
            ToolName = "Selenium",
            Profile = new() { Phone = "0901000002" }
        };
        var cuong = new Developer
        {
            FullName = "Lê Hoàng Cường",
            Email = "cuong@pm.vn",
            MainLanguage = "Java",
            YearsOfExperience = 3,
            Profile = new() { Phone = "0901000003" }
        };
        var dung = new Tester
        {
            FullName = "Phạm Thu Dung",
            Email = "dung@pm.vn",
            ToolName = "Playwright"
        }; // chưa có hồ sơ

        db.Customers.AddRange(fpt, viettel, vnpt);
        db.Resources.AddRange(an, binh, cuong, dung); // DbSet lớp cơ sở nhận mọi lớp con
        db.Projects.AddRange(crm, erp, bank);

        // N – N có payload: thêm trực tiếp entity nối
        db.Assignments.AddRange(
            new Assignment { Project = crm, Resource = an, Role = "Tech Lead", JoinedOn = new DateTime(2026, 1, 5) },
            new Assignment { Project = crm, Resource = binh, Role = "QA", JoinedOn = new DateTime(2026, 1, 10) },
            new Assignment { Project = erp, Resource = an, Role = "Developer", JoinedOn = new DateTime(2026, 3, 1) },
            new Assignment { Project = erp, Resource = cuong, Role = "Developer", JoinedOn = new DateTime(2026, 3, 1) },
            new Assignment { Project = bank, Resource = cuong, Role = "Tech Lead", JoinedOn = new DateTime(2026, 2, 15) },
            new Assignment { Project = bank, Resource = dung, Role = "QA", JoinedOn = new DateTime(2026, 2, 20) });

        await db.SaveChangesAsync(ct); // một transaction duy nhất
    }
}