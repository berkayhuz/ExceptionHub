using ExceptionHub.Abstractions;
using ExceptionHub.Configuration;
using ExceptionHub.Core.Mapping;
using ExceptionHub.Http;
using ExceptionHub.Mediator;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ExceptionHub.DependencyInjection;

/// <summary>
/// Provides extension methods for registering ExceptionHub services into the application's dependency injection container and middleware pipeline.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core ExceptionHub services into the dependency injection container.
    /// This includes default exception mappers, options configuration, the global exception handler,
    /// and MediatR pipeline behavior support.
    /// </summary>
    /// <param name="services">The service collection to register into.</param>
    /// <param name="configure">
    /// An optional delegate to configure custom exception mappings via <see cref="ExceptionHubOptions"/>.
    /// </param>
    /// <returns>The modified <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddExceptionHub(this IServiceCollection services, Action<ExceptionHubOptions>? configure = null)
    {
        var options = new ExceptionHubOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IExceptionMapper, DefaultExceptionMapper>();
        services.AddSingleton<GlobalExceptionHandler>();

        services.AddSingleton<DefaultExceptionMapper>();
        services.AddSingleton<OptionsExceptionMapper>();
        services.AddSingleton<IExceptionMapper>(sp =>
            new CompositeExceptionMapper(sp.GetServices<IExceptionMapper>()));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

        return services;
    }

    /// <summary>
    /// Adds the ExceptionHub HTTP middleware to the ASP.NET Core pipeline.
    /// This integrates the <see cref="GlobalExceptionHandler"/> with the request lifecycle,
    /// enabling centralized exception mapping and structured error responses.
    /// </summary>
    /// <param name="app">The application builder instance.</param>
    /// <returns>The modified <see cref="IApplicationBuilder"/> instance.</returns>
    public static IApplicationBuilder UseExceptionHub(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(static builder => { });

        app.Use(async (context, next) =>
        {
            var handler = context.RequestServices.GetRequiredService<GlobalExceptionHandler>();

            try
            {
                await next.Invoke();
            }
            catch (Exception ex)
            {
                var handled = await handler.TryHandleAsync(context, ex, context.RequestAborted);

                if (!handled)
                    throw;
            }
        });

        return app;
    }
}
