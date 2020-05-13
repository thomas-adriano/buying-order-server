

using buying_order_server.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentMigrator.Runner;
using buying_order_server.Data.Migrations;

namespace buying_order_server.Infrastructure.Installers
{
    public class RegisterDBMigrator : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    // Add SQLite support to FluentMigrator
                    .AddPostgres()
                    // Set the connection string
                    .WithGlobalConnectionString(configuration.GetConnectionString("PostgreSQLConnectionString"))
                    // Define the assembly containing the migrations
                    .ScanIn(typeof(CreateTables).Assembly).For.Migrations())
                //.AddScoped<PostgresQuoter, NoQuoteQuoter>()
                // Enable logging to console in the FluentMigrator way
                .AddLogging(lb => lb.AddFluentMigratorConsole())
                // Build the service provider
                .BuildServiceProvider(false);
        }
    }
}
