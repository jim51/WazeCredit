using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WazeCredit.Data.Repository;
using WazeCredit.Data.Repository.IRepository;
using WazeCredit.Models;
using WazeCredit.Service;
using WazeCredit.Service.LifeTimeExample;

namespace WazeCredit.Utiltity.DI_Config
{ 
    public static class ConfigureDIServices
    {
        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
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
            services.AddScoped<IUnitOfWork, UnitOfWork>();
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

            return services;
        }
    }
}
