using Microsoft.Extensions.DependencyInjection;
using CouponMaster.Abstraction;
using CouponMaster.Business;
using CouponMaster.Repository;
using CouponMaster.Logging;

namespace CouponMaster.DependencyResolver
{
    public static class ServiceCollectionExtension
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // When someone asks for IManager, give them CouponManager
            services.AddTransient<ICouponManager, CouponManager>();

            // When someone asks for IRepository, give them CouponRepository
            services.AddTransient<ICouponRepository, CouponRepository>();

            // Note: "Transient" means a new instance is created every time.
            // For heavy services, we might use "Scoped" (once per HTTP request).

            services.AddSingleton<ILoggerManager, LoggerManager>();
            // Singleton because we only need one logger instance for the whole app.
        }
    }
}