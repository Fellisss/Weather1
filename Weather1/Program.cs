using Microsoft.EntityFrameworkCore;
using Weather1.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Строка подключения (через appsettings.json или значение по умолчанию)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=Weather1Db;Trusted_Connection=True;MultipleActiveResultSets=true";

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Маршрут по умолчанию: Observations/Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Observations}/{action=Index}/{id?}");

app.Run();