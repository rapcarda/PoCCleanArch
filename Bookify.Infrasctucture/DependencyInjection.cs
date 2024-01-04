using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Email;
using Bookify.Infrasctucture.Clock;
using Bookify.Infrasctucture.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrasctucture;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IEmailService, EmailService>();
        
        return services;
    }
}
