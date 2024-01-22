using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PalmVillas;
using PalmVillas.DbServices;
using PalmVillas.Models;
using NLog;
using NLog.Web;
using System.Configuration;
using System.Data.SQLite;
using NLog.Fluent;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using PalmVillas.Areas.Identity;
using System.Net;
using PalmVillas.Domain;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Data.Entity;




var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PalmContextConnection") ?? throw new InvalidOperationException("Connection string 'PalmContextConnection' not found.");

var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Data\\palm.db");
//var connectionString = "Data Source=" + path;
builder.Services.AddDbContext<PalmContext>(options => options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<User>(
    options => { 
        options.SignIn.RequireConfirmedAccount = false;
       
    }) 
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PalmContext>();

// Add services to the container.
var services = builder.Services;

services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(365); // Set the cookie expiration time
    options.SlidingExpiration = true; // Renew the cookie expiration on each request
    
});

services.AddAuthorization(options =>
{   
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ElevatedRights", policy => policy.RequireRole("Villa Manager","Admin"));
});


StartUtil.EnsureTableExists();
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

// NLog: Setup NLog for Dependency injection
builder.Logging.ClearProviders();
builder.Host.UseNLog();
logger.Info("Logging working");


services.AddDbContext<PalmContext>(options =>
          options.UseSqlite(connectionString));



services.AddControllersWithViews().AddRazorRuntimeCompilation();

services.AddScoped<IAccountDbService, AccountDbService>();

services.AddScoped<BookingUtility>();
services.AddScoped<IVillaDbService, VillaDbService>();
services.AddScoped<IBookingDbService, BookingDbService>();
services.AddRazorPages();

services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");


var app = builder.Build();

var sp = services.BuildServiceProvider();
var context = sp.GetService<PalmContext>();
CheckGuestExists(context);

void CheckGuestExists(PalmContext? context)
{
    if (context.Users.Find("guest") == null)
    {
        var guest = new User()
        {
            Id = "guest",
            Name = "Guest",
            Email = "guest@gmail.com"
        };
        context.Users.Add(guest);
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public static class StartUtil
{
    public static void EnsureTableExists()
    {       
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Data\\palm.db");
        var connectionString = "Data Source=" + path;       

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        using (SQLiteCommand command = new SQLiteCommand(
            "CREATE TABLE if not exists Log (Timestamp TEXT, Loglevel TEXT, Callsite TEXT, Message TEXT, Exception TEXT)",
            connection))
        {
            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
