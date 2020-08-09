using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WazeCredit.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WazeCredit.Service;
using WazeCredit.Utiltity.AppSettingsClasses;
using WazeCredit.Utiltity.DI_Config;
using WazeCredit.Middleware;
using WazeCredit.Service.LifeTimeExample;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WazeCredit.Models;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

namespace WazeCredit
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();



            services.AddAppSettingsConfig(Configuration);

            //services.AddScoped<IValidateionChecker, AddressValidationChecker>();
            //services.AddScoped<IValidateionChecker, CreditValidationChecker>();
            // 改用TryAddEnumerable避免重複註冊
            //services.TryAddEnumerable(ServiceDescriptor.Scoped<IValidateionChecker,AddressValidationChecker>());
            //services.TryAddEnumerable(ServiceDescriptor.Scoped<IValidateionChecker, CreditValidationChecker>());
            // 改用TryAddEnumerable避免重複註冊=>更優雅寫的法
            services.TryAddEnumerable(new[]
            {
                ServiceDescriptor.Scoped<IValidateionChecker,AddressValidationChecker>(),
                ServiceDescriptor.Scoped<IValidateionChecker, CreditValidationChecker>()
            });

            services.AddScoped<ICreditValidator, CreditValidator>();

            services.AddTransient<TransientService>();
            services.AddSingleton<SingletionService>();
            services.AddScoped<ScopedService>();

            services.AddScoped<CreditApprovedLow>();
            services.AddScoped<CreditApprovedHigh>();

            services.AddScoped<Func<CreditApprovedEnum, ICreditApproved>>(ServiceProvider => range =>
                {
                    switch (range)
                    {
                        case CreditApprovedEnum.Low:
                            return ServiceProvider.GetService<CreditApprovedLow>();
                        case CreditApprovedEnum.High:
                            return ServiceProvider.GetService<CreditApprovedHigh>();
                        default:
                            return ServiceProvider.GetService<CreditApprovedLow>();
                    };
                }
            );

            /// 增加IMarketForecaster注入服務
            services.AddTransient<IMarketForecaster, MarketForecasterV2>();
            /// 用取代的方式取代前面註冊
            services.Replace(ServiceDescriptor.Transient<IMarketForecaster, MarketForecaster>());

            /// 刪除前面註冊
            //services.RemoveAll<IMarketForecaster>();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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
            loggerFactory.AddFile("logs/creditApp-log-{Date}.txt");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<CustomMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
