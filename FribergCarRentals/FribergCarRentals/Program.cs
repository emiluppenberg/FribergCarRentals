using FribergCarRentals.Data;
using FribergCarRentals.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews()
                .AddCookieTempDataProvider();

            builder.Services.AddScoped<IRepository<Admin>, AdminRepository>();
            builder.Services.AddScoped<IRepository<Kund>, KundRepository>();
            builder.Services.AddScoped<IBilRepository, BilRepository>();
            builder.Services.AddScoped<IBokningRepository, BokningRepository>();
            builder.Services.AddSingleton<IUserService, UserService>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication("MyCookie")
                .AddCookie("MyCookie", options =>
                {
                    options.SlidingExpiration = false;
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build()
            .GetConnectionString("AppDbContext")));

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
