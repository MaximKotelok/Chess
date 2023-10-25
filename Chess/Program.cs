using DataAccess.Data;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.SignalR;
using Utils;
using Humanizer;
using Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();


builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});



builder.Services.AddSignalR();  

builder.Services.AddDbContext<ApplicationDbContext>(
        options => {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"));
        }
        );


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

builder.Services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddSession();
builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(
	opts => {
		opts.SignIn.RequireConfirmedEmail = false;
		opts.SignIn.RequireConfirmedAccount = false;
	}
	);

builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = $"/Identity/Account/Login";
	options.LogoutPath = $"/Identity/Account/Logout";
	options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

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

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
SeedDatabase();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalR>("/signalr");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Game}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
	using (var scope = app.Services.CreateScope())
	{
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}

}