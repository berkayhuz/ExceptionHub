using ExceptionHub.Abstractions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExceptionHub.Workers;

/// <summary>
/// Provides extension methods for registering hosted background services with centralized exception classification using ExceptionHub.
/// </summary>
public static class HostedServiceRegistrationExtensions
{
    /// <summary>
    /// Registers the specified <typeparamref name="TService"/> as a background service,
    /// and wraps it with <see cref="ExceptionHandlingServiceDecorator{T}"/> to capture and classify
    /// any unhandled exceptions using <see cref="IExceptionMapper"/>.
    /// </summary>
    /// <typeparam name="TService">The concrete hosted service type to register.</typeparam>
    /// <param name="services">The service collection to register with.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    /// <remarks>
    /// This method ensures that the decorated service is only started once by registering the inner service
    /// as itself (not as <see cref="IHostedService"/>), and registering the decorator as the actual hosted service.
    /// </remarks>
    public static IServiceCollection AddExceptionHubHostedService<TService>(this IServiceCollection services)
        where TService : class, IHostedService
    {
        services.AddSingleton<TService>();

        services.AddSingleton<IHostedService>(sp =>
        {
            var inner = sp.GetRequiredService<TService>();
            var mapper = sp.GetRequiredService<IExceptionMapper>();
            return new ExceptionHandlingServiceDecorator<TService>(inner, mapper);
        });

        return services;
    }
}
