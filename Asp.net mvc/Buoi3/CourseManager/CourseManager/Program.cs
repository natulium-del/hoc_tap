var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
    name: "khoa-hoc-detail",
    pattern: "khoa-hoc/chi-tiet{id:int:min(1)}",
    defaults: new { controller = "Course", action = "Details" });

app.MapControllerRoute(
    name: "khoa-hoc-category",
    pattern: "khoa-hoc/{category:alpha}",
    defaults: new { controller = "Course", action = "ByCategory" });

app.MapControllerRoute(
    name: "dang-ky",
    pattern: "dang-ky",
    defaults: new { controller = "Course", action = "Register" });

app.MapControllerRoute(
    name: "danh-sach",
    pattern: "khoa-hoc",
    defaults: new { controller = "Course", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "khoa-hoc-code",
    pattern: "khoa-hoc/ma/{code:regex(^KH-\\d{{4}}$)}",
    defaults: new { controller = "Course", action = "ByCode" });

app.MapControllerRoute(
    name: "khoa-hoc-page",
    pattern: "khoa-hoc/trang/{page:int:min(1)}",
    defaults: new { controller = "Course", action = "Index" });

app.Run();
