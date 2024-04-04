using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json.Serialization;
using WMBA_4.Data;
using WMBA_4.Utilities;
using WMBA_4.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDbContext<WMBA_4_Context>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

//For email service configuration
builder.Services.AddSingleton<IEmailConfiguration>(builder.Configuration
    .GetSection("EmailConfiguration").Get<EmailConfiguration>());
//For the Identity System
builder.Services.AddTransient<IEmailSender, EmailSender>();

//Email with added methods for production use.
builder.Services.AddTransient<IMyEmailSender, MyEmailSender>(); 

builder.Services.AddControllersWithViews();

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//    .RequireAuthenticatedUser()
//        .Build();
//});
//To give access to IHttpContextAccessor for Audit Data with IAuditable
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddControllersWithViews();

//For email service configuration
builder.Services.AddSingleton<IEmailConfiguration>(builder.Configuration
    .GetSection("EmailConfiguration").Get<EmailConfiguration>());

//For the Identity System
builder.Services.AddTransient<IEmailSender, EmailSender>();

//Email with added methods for production use.
builder.Services.AddTransient<IMyEmailSender, MyEmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "activate",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "activate",
    pattern: "Player/Activate/{id}",
    defaults: new { controller = "Player", action = "Activate" });

app.MapControllerRoute(
    name: "activate",
    pattern: "Team/Activate/{id}",
    defaults: new { controller = "Team", action = "Activate" });

app.MapRazorPages();


//Seed Sata
WMBA4Initializer.Seed(app);
ApplicationDbInitializer.Seed(app);

app.Run();
