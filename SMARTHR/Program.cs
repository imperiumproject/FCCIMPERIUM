using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using SmartHR.Application.D365ApiClient;
using SmartHR.Application.Interfaces;
using SmartHR.Application.Services;
using SmartHR.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .WriteTo.File("Logs/app.log", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information();
});

// ?? Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

// ?? Authorization
builder.Services.AddAuthorization();

// ?? MVC + Views
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

// ?? Services and DI
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddTransient<IEmployeeService, EmployeeService>();
builder.Services.AddTransient<ILeaveService, LeaveService>();
builder.Services.AddTransient<ID365ApiClient, MockD365ApiClient>();
builder.Services.AddTransient<IMenuItemService, MenuItemService>();
builder.Services.AddTransient<ID365AccountClientApi, D365AccountClientApi>();
builder.Services.AddTransient<IAuthentications, AuthenticationsServices>();

// ?? Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
    logging.AddFilter("Microsoft.AspNetCore", LogLevel.Debug);
});


// Register session services (uses in-memory cache by default)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Prevent client-side access
    options.Cookie.IsEssential = true; // Required for GDPR compliance
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use HTTPS only
});


var app = builder.Build();

// ?? Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ?? Auth
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
// ?? Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authentication}/{action=Login}/{id?}");

app.Run();
