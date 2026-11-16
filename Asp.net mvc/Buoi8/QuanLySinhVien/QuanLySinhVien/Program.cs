using QuanLySinhVien.Middlewares;
using System.Diagnostics;
using QuanLySinhVien.Services;
using Microsoft.EntityFrameworkCore;
using QuanLySinhVien.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký DbContext (Gộp 2 lần đăng ký làm 1, thêm cấu hình Log cho Dev)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information)
               .EnableSensitiveDataLogging()
               .EnableDetailedErrors();
    }
});

// 2. Đăng ký MVC (Chỉ cần 1 lần)
builder.Services.AddControllersWithViews();

// 3. Đăng ký Service (Đã xóa AddSingleton, giữ lại AddScoped)
builder.Services.AddScoped<ISinhVienService, SinhVienService>();

var app = builder.Build();

// 4. Khởi tạo dữ liệu mẫu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Seed(db);
}

// Configure the HTTP request pipeline.
// Các Middleware tự viết
app.UseRequestTiming();
app.UseBlockBot();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
}

app.UseRequestCounter();

app.UseRouting();
app.UseAuthorization();

app.Map("/bao-tri", branch => branch.Run(async context =>
{
    context.Response.StatusCode = 503;
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(
        "<h2>Hệ thống đang bảo trì. Vui lòng quay lại sau.</h2>");
}));

app.MapStaticAssets();

// Endpoint /thong-ke
app.MapGet("/thong-ke", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(
        $"<h2>Tổng số request đã phục vụ: {RequestCounterMiddleware.RequestCount}</h2>");
});

app.MapControllerRoute(
    name: "sinhVienChiTiet",
    pattern: "sv/{id:int:min(1)}",
    defaults: new { controller = "SinhVien", action = "Detail" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();