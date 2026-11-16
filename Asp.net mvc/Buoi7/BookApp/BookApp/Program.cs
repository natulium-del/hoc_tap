using BookApp.Data;
using BookApp.Services;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BookDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseSeeding((context, _) =>
    {
        SeedData.SeedAsync((BookDbContext)context, false, CancellationToken.None)
                .GetAwaiter().GetResult();
    });
    options.UseAsyncSeeding(async (context, _, ct) =>
    await SeedData.SeedAsync((BookDbContext)context, false, ct));

    if (builder.Environment.IsDevelopment())
    {
        options.LogTo(Console.WriteLine, LogLevel.Information) // IN CÂU SQL – quan trọng cho cả bài
               .EnableSensitiveDataLogging();
    }
});

builder.Services.AddScoped<BookListService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider
        .GetRequiredService<BookDbContext>()
        .Database
        .MigrateAsync();
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
