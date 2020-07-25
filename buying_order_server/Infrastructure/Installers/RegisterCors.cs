using buying_order_server.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace buying_order_server.Infrastructure.Installers
{
    internal class RegisterCors : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            //Configure CORS to allow any origin, header and method. 
            //Change the CORS policy based on your requirements.
            //More info see: https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.0
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                builder => builder.WithOrigins(config["CORS:Origin1"], config["CORS:Origin2"], config["CORS:Origin3"])
                            .WithMethods("GET", "POST", "PUT", "DELETE")
                            .AllowAnyHeader()
                            .AllowCredentials()
                );
            });
        }
    }
}
