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
    name: "blog",
    pattern: "blog/{year}/{month}/{slug}",
    defaults: new { controller = "Blog", action = "Post" });

app.MapControllerRoute(
    name: "shortcut",
    pattern: "lien-he",
    defaults: new { controller = "Home", action = "Contact" });

app.MapControllerRoute(
    name:"productDetails",
    pattern: "product/{id:int:min(1)}",
    defaults: new { controller = "Product", action = "Details" }
    );

app.MapControllerRoute(
    name: "category",
    pattern: "category/{name:alpha:minlength(3)}",
    defaults: new { controller = "Product", action = "Category" }
    );

app.MapControllerRoute(
    name:"archive",
    pattern: "archive/{year:range(2000,2030)}/{month:int:range(1,12)}",
    defaults: new { controller = "Blog", action = "Archive" }
    );

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
