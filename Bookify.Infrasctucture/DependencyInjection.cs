using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Email;
using Bookify.Infrasctucture.Clock;
using Bookify.Infrasctucture.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrasctucture;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<IEmailService, EmailService>();

        var connectionString = configuration.GetConnectionString("DataBase") ??
            throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            // O EF Core por convenção usa TitleCase para nomes de tabelas e colunas, mas o postgre usa SnakeCase.
            // Para facilitar a conversão, instalar o package EFCore.NameingConventions e utilizar o método UseSnakeCaseNamingConvention()
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });
        
        return services;
    }
}
