using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectManager.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseAsyncSeeding(SeedData.SeedAsync); // [MỚI] đăng ký hàm seed

    if (builder.Environment.IsDevelopment())
    {
        options
            .LogTo(Console.WriteLine, LogLevel.Information) // in câu SQL ra Console
            .EnableSensitiveDataLogging();                  // hiện giá trị tham số (chỉ dùng khi dev)
    }
});
var app = builder.Build();

// [MỚI] áp dụng migration còn thiếu và chạy seeding khi khởi động
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService <AppDbContext> ();
    await db.Database.MigrateAsync();
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
