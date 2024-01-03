﻿using Bookify.Application.Abstractions.Behaviors;
using Bookify.Domain.Bookings.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            configuration.AddOpenBehavior(typeof(LogginBehavior<,>));
            configuration.AddBehavior(typeof(ValidationBehavior<,>));
        });

        // Nesta configuração esta dizendo para o FluentValidation varrer todo o assembly informado e registrar toda validação como IValidator (que esta no ValidatorBehavior)
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient<PricingService>();

        return services;
    }
}

