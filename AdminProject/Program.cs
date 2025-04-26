using AdminProject.Authorization;
using AdminProject.Business.Managers;
using AdminProject.Data.Context;
using AdminProject.Data.Domain.Users;
using AdminProject.Data.Repositories;
using AdminProject.Data.Seeds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AccountClaimManager.UserEditClaim, policy => policy.RequireClaim("Permission", AccountClaimManager.UserEditClaim));
    options.AddPolicy(AccountClaimManager.AdminClaim, policy => policy.RequireClaim("Permission", AccountClaimManager.AdminClaim));
    options.AddPolicy(AccountClaimManager.UserAddClaim, policy => policy.RequireClaim("Permission", AccountClaimManager.UserAddClaim));
    options.AddPolicy(AccountClaimManager.UserDeleteClaim, policy => policy.RequireClaim("Permission", AccountClaimManager.UserDeleteClaim));
    options.AddPolicy(AccountClaimManager.UserViewClaim, policy => policy.RequireClaim("Permission", AccountClaimManager.UserViewClaim));
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services
    .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
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
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<UserAccountManager>();
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    await dbContext.Database.MigrateAsync();

    await IdentitySeeder.SeedRolesAndAdminAsync(services);
}


app.Run();