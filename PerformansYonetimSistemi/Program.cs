using Microsoft.EntityFrameworkCore;
using PerformansYonetimSistemi.Helper.Database;

var builder = WebApplication.CreateBuilder(args);
// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false);


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
