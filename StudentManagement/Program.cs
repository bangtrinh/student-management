using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using OfficeOpenXml;
using StudentManagement.Hubs;
using StudentManagement.Models;
using StudentManagement.Repositories;
using StudentManagement.Services;
using System.Globalization;
using StudentManagement.Authorization;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


// Localization cấu hình Resource folder
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Database

builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 🟢 Thêm Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Manage/ChangePassword");
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<StudentDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Razor Pages với hỗ trợ localization
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Manage/ChangePassword");
})
.AddViewLocalization()
.AddDataAnnotationsLocalization();

// Controllers + Views + Localization
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Session

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole(SD.Role_Admin));

    options.AddPolicy("RequireTeacherRole", policy =>
        policy.RequireRole(SD.Role_Teacher));

    options.AddPolicy("RequireStudentRole", policy =>
        policy.RequireRole(SD.Role_Student));

    options.AddPolicy("RequireAdminOrTeacher", policy =>
        policy.RequireRole(SD.Role_Admin, SD.Role_Teacher));

    options.AddPolicy("RequireAdminOrStudent", policy =>
    policy.RequireRole(SD.Role_Admin, SD.Role_Student));

    options.AddPolicy("AllowAll", policy =>
    policy.RequireRole(SD.Role_Admin, SD.Role_Student, SD.Role_Teacher));

    options.AddPolicy("GmailAndAdminOnly", policy =>
    {
        policy.RequireRole("Admin");
        policy.Requirements.Add(new GmailEmailRequirement());
    });


}); 

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IMajorRepository, MajorRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IScheduleRepository, ScheduleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddSingleton<IAuthorizationHandler, GmailEmailHandler>();


builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Seed dữ liệu nếu chưa có user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var userCount = userManager.Users.Count();
        if (userCount == 0)
        {
            await SeedData.Initialize(services);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}
// ✅ Cấu hình Localization Middleware
var supportedCultures = new[] { new CultureInfo("vi"), new CultureInfo("en") };

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};

// 🔥 Ưu tiên lấy ngôn ngữ từ cookie do LanguageController đặt
localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseRequestLocalization(localizationOptions); // ✅ Đặt ngay sau UseRouting

app.UseAuthentication();

app.UseRequestLocalization(localizationOptions);
app.UseAuthorization();
app.UseSession();

app.MapRazorPages();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapHub<ChatHub>("/chathub");

app.Run();
