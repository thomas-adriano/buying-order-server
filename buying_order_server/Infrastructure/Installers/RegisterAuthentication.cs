using buying_order_server.Contracts;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using System.Threading.Tasks;

namespace buying_order_server.Infrastructure.Installers
{
    internal class RegisterAutentication : IServiceRegistration
    {
        public void RegisterAppServices(IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            services.AddAuthentication(
                    CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate(options =>
                    {
                        options.AllowedCertificateTypes = env.IsDevelopment() ? CertificateTypes.All : CertificateTypes.Chained;
                        options.Events = new CertificateAuthenticationEvents
                        {
                            OnCertificateValidated = context =>
                            {
                                var claims = new[]
                                {
                                    new Claim(
                                        ClaimTypes.NameIdentifier,
                                        context.ClientCertificate.Subject,
                                        ClaimValueTypes.String,
                                        context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name,
                                        context.ClientCertificate.Subject,
                                        ClaimValueTypes.String,
                                        context.Options.ClaimsIssuer)
                                };

                                context.Principal = new ClaimsPrincipal(
                                    new ClaimsIdentity(claims, context.Scheme.Name));
                                context.Success();

                                return Task.CompletedTask;
                            }
                        };
                    });
        }
    }
}
