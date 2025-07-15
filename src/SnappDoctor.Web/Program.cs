using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnappDoctor.Application.Contracts;
using SnappDoctor.Core.Entities;
using SnappDoctor.Infrastructure.Data;
using SnappDoctor.Infrastructure.Repositories;
using SnappDoctor.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    
    // User settings
    options.User.RequireUniqueEmail = false;
    options.User.AllowedUserNameCharacters = "0123456789";
    
    // SignIn settings
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

// Add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOtpRepository, OtpRepository>();
builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();

// Add services
builder.Services.AddScoped<ISmsService, KavenegarSmsService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IScheduleService, ScheduleService>();

// Add MVC
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run(); 