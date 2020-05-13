using buying_order_server.Contracts;
using buying_order_server.Infrastructure.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;

namespace buying_order_server.Infrastructure.Installers
{
    internal class RegisterHealthChecks : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration config)
        {
            //Register HealthChecks and UI
            services.AddHealthChecks()
                    .AddCheck("Google Ping", new PingHealthCheck("www.google.com", 300))
                    .AddUrlGroup(new Uri(config["ApiResourceBaseUrls:EcosysApi"]),
                                name: "Ecosys Api",
                                failureStatus: HealthStatus.Degraded)
                    .AddNpgSql(config["ConnectionStrings:PostgreSQLConnectionString"],
                                healthQuery: "SELECT 1;",
                                name: "PostgreSQL",
                                failureStatus: HealthStatus.Degraded);

            services.AddHealthChecksUI();
        }
    }
}
