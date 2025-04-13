using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VacationManager.Data;
using VacationManager.Services;
using VacationManager.Models;
using Microsoft.AspNetCore.Http;
using VacationManager.Services;
using VacationManager.Settings;


var builder = WebApplication.CreateBuilder(args);

// ⬚ Добавяне на контролери и изгледи
builder.Services.AddControllersWithViews();

// ⬚ Конфигуриране на EF Core с връзка към базата данни
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ⬚ Регистриране на Identity с настройки
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ✅ Cookie политика — оправя проблема със SameSite при HTTPS/localhost
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.Secure = CookieSecurePolicy.Always;
});

// ⬚ Конфигуриране на Cookie Authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// ⬚ Регистриране на услугите за Dependency Injection
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

// ✅ Email услуга (SMTP)
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

var app = builder.Build();

// ✅ Създаване на роли и администратор чрез IdentitySeeder
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAdminAsync(scope.ServiceProvider);
}

// ⬚ Конфигуриране на HTTP pipeline-а
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ✅ Cookie политика преди автентикация
app.UseCookiePolicy();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ⬚ Маршрутизация
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "auth",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "project",
    pattern: "{controller=Project}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "leaveRequest",
    pattern: "{controller=LeaveRequest}/{action=Index}/{id?}");

app.Run();
