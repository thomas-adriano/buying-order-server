using buying_order_server.Contracts;
using buying_order_server.DTO.Request;
using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace buying_order_server.Infrastructure.Installers
{
    internal class RegisterModelValidators : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            //Register DTO Validators
            services.AddTransient<IValidator<AppConfigurationDTO>, CreateOrUpdateAppConfigurationRequestValidator>();
            services.AddTransient<IValidator<PostponedOrderDTO>, PostponeOrderRequestValidator>();

            //Disable Automatic Model State Validation built-in to ASP.NET Core
            services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
        }
    }
}
