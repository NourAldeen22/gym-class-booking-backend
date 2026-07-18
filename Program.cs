// 🌟 أضف أسطر الـ global using هنا في أول الملف تماماً فوق كل شيء
global using userMangment.Models;       // ليتعرف المشروع كله (بما فيه الـ Identity) على ApplicationUser
global using userMangment.Interfaces;   // لكي ترى السيرفس والكونترولر الإنترفيس في مجلدها الجديد تلقائياً


using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using userMangment;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Runtime.CompilerServices;
using userMangment.Services;
// using userMangment.Models;
// using userMangment.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IGymService,GymService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IUserService,UserServiece>();
builder.Services.AddScoped<IAdmminService,UserServiece>();




builder.Services.AddDbContext<ApplicationDbContext>(options=> 
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>{
 options.SignIn.RequireConfirmedAccount = false; options.Lockout.MaxFailedAccessAttempts= 5;}).
 AddRoles<IdentityRole>().
 AddEntityFrameworkStores<ApplicationDbContext>();



// builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<ApplicationDbContext>();
// builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// using(var scope = app.Services.CreateScope())
// {

//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

//     if (!await roleManager.RoleExistsAsync("Admin"))
//     {
//         await roleManager.CreateAsync(new IdentityRole("Admin"));
//     }

//     string adminEmail = "user@test.com";
//     var adminUser = await userManager.FindByEmailAsync(adminEmail);

//     if(adminUser != null)
//     {
//         if(!await userManager.IsInRoleAsync(adminUser,"Admin"))
//         {
//             await userManager.AddToRoleAsync(adminUser,"Admin");
//         }
//     }





// }

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();


app.Run();
