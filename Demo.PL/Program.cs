using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Contexts;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.Mappers;
using Demo.PL.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Demo.PL
{
    public class Program
    {
        // Entry Point
        public static void Main(string[] args)
        {
            // CreateHostBuilder().Build().Run();

            var builder = WebApplication.CreateBuilder(args);

            #region Configure Services That Allow Dependency Injection
            

            builder.Services.AddControllersWithViews(); // MVC Services

            builder.Services.AddDbContext<MVCAppG01DbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });


            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddAutoMapper(M => M.AddProfile(new EmployeeProfile()));
            builder.Services.AddAutoMapper(M => M.AddProfile(new UserProfile()));
            builder.Services.AddAutoMapper(M => M.AddProfile(new RoleProfile()));



            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services.AddTransient<IEmailSettings, EmailSettings>();


			builder.Services.Configure<TwillioSettings>(builder.Configuration.GetSection("Twilio"));

			builder.Services.AddTransient<Isms, SMS>();

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
                }
            ).AddGoogle( o =>
            {
                IConfiguration GoogleAuth = builder.Configuration.GetSection("Authentication:Google");
                o.ClientId = GoogleAuth["ClientId"];
                o.ClientSecret = GoogleAuth["ClientSecret"];

			});
           



			builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = true; // @#$%
                options.Password.RequireUppercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                //options.Password.RequiredLength = 4;
            })

                .AddEntityFrameworkStores<MVCAppG01DbContext>()
                .AddDefaultTokenProviders();
            


            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "Account/Login";
					options.AccessDeniedPath = "Home/Error";
                });
           


			#endregion


			var app = builder.Build();

            #region Configure Http Request Pipelines


            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            #endregion


            app.Run();
        }

    }
}
