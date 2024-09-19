using System.Configuration;
using Domain.Entity;
using Infrastructure.Data;
using Infrastructure.IRepository;
using Infrastructure.IRepository.ServicesRepository;
using Infrastructure.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddDbContext<FreeBookDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Bookconnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Bookconnection"))
    )
);
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<FreeBookDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequiredLength = 8;
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Admin";
    options.AccessDeniedPath = "/Admin/Home/Denied";
});

builder.Services.AddScoped<IServicesRepository<Category>, ServicesCategory>();
builder.Services.AddScoped<IServicesLogRepository<LogCategory>, ServicesLogCategory>();


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
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Accounts}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();