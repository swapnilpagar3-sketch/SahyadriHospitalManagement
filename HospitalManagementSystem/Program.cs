using System.IO;
using HospitalManagementSystem.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Data Protection Keys local directory (ensures key persistence works in all environments)
var keysFolder = Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys");
Directory.CreateDirectory(keysFolder);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysFolder))
    .SetApplicationName("SahyadriHospitalManagement");

// Database configuration
var dbProvider = builder.Configuration["DatabaseProvider"] ?? "Sqlite";
if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
{
    var sqlConn = builder.Configuration.GetConnectionString("SqlServerConnection");
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(sqlConn));
}
else
{
    var sqliteConn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=HospitalManagement.db";
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConn));
}

// Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "HospitalManagementSystem.Auth";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireDoctor", policy => policy.RequireRole("Doctor", "Admin"));
    options.AddPolicy("RequireNurse", policy => policy.RequireRole("Nurse", "Admin"));
    options.AddPolicy("RequireWarden", policy => policy.RequireRole("Warden", "Admin"));
    options.AddPolicy("RequirePatient", policy => policy.RequireRole("Patient", "Admin"));
    options.AddPolicy("RequireStaff", policy => policy.RequireRole("Admin", "Doctor", "Nurse", "Warden"));
});

// MVC & Session
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
