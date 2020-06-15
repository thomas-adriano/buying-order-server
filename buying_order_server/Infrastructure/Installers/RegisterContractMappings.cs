using buying_order_server.Contracts;
using buying_order_server.Data.Repository;
using buying_order_server.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace buying_order_server.Infrastructure.Installers
{
    internal class RegisterContractMappings : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IAppExecutionStatusManager, AppExecutionStatusManager>();
            services.AddTransient<IPostponedOrderRepository, PostponedOrderRepository>();
            services.AddTransient<IAppConfigurationRepository, AppConfigurationRepository>();
            services.AddSingleton<ICronJob, EmailCronJob>();
            services.AddSingleton<IBuyingOrdersManager, BuyingOrdersManager>();
        }
    }
}
