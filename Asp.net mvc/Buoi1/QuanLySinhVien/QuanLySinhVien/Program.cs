using QuanLySinhVien.Middlewares;
using System.Diagnostics;
var builder = WebApplication.CreateBuilder(args); // Tạo builder

// Add services to the container.
builder.Services.AddControllersWithViews(); //Đăng kí dịch vụ

var app = builder.Build();

//app.Use(async (context, next) =>
//{
//    var sw = Stopwatch.StartNew();
//    Console.WriteLine($"--> VÀO : {context.Request.Method} " +
//                      $"{context.Request.Path}");
//    await next(); // chuyển tiếp cho middleware kế tiếp
//    sw.Stop();
//    Console.WriteLine($"<-- RA : {context.Response.StatusCode} " +
//                      $"({sw.ElapsedMilliseconds} ms)");
//});

app.UseRequestTiming();
app.UseBlockBot();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())  // Dùng pipeline
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

// Endpoint /thong-ke — trả về số request
app.MapGet("/thong-ke", async context =>
{
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(
        $"<h2>Tổng số request đã phục vụ: {RequestCounterMiddleware.RequestCount}</h2>");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run(); //Chạy ứng dụng
