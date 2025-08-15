using Fartak.Services;
using Fartak.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fartak
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddScoped<IGetAdmin, GetAdmin>();
            services.AddScoped<IMe, Me>();
            services.AddHttpContextAccessor();
            services.AddDbContext<DataBase>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.Configure<JwtSettings>(Configuration.GetSection("JwtSettings"));


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
         .AddJwtBearer(options =>
         {
             options.TokenValidationParameters = new TokenValidationParameters
             {
                 ValidateIssuerSigningKey = true,
                 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecretKeyForJwtAuthenticationInAspNetCore")),
                 ValidateIssuer = false,
                 ValidateAudience = false,
                 ClockSkew = TimeSpan.Zero // جلوگیری از تاخیر در انقضا
            };

             //اینتر فیس ایس ادمین

             // پیکربندی برای خواندن توکن از کوکی
             options.Events = new JwtBearerEvents
             {
                 OnMessageReceived = context =>
                 {
                    // بررسی اینکه آیا توکن در کوکی وجود دارد
                    var token = context.Request.Cookies["jwt"];
                     if (!string.IsNullOrEmpty(token))
                     {
                         context.Token = token;
                     }
                     return Task.CompletedTask;
                 }
             };

             services.AddControllers();


         });


            // اگر از کوکی‌ها استفاده می‌کنید
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // برای HTTPS
    });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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

            app.UseCors("AllowAllOrigins");

            app.UseStatusCodePagesWithReExecute("/Error/Error/{0}");

            //JWT Token
            app.UseAuthentication();
            app.UseAuthorization();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Product}/{action=AllProducts}/{id?}");
            });
        }

    }
}
