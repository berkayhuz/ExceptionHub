using ExceptionHub.Messaging.Filters;

using MassTransit;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExceptionHub.Messaging.Extensions;

/// <summary>
/// Provides extension methods for integrating ExceptionHub with MassTransit consumers.
/// This enables centralized exception classification during message consumption.
/// </summary>
public static class MassTransitConfiguratorExtensions
{
    /// <summary>
    /// Configures MassTransit to use ExceptionHub's <see cref="ExceptionHandlingConsumeFilter{T}"/>
    /// for all message consumers, enabling structured exception handling.
    /// </summary>
    /// <param name="cfg">The bus registration configurator.</param>
    /// <returns>The same <see cref="IBusRegistrationConfigurator"/> instance for fluent chaining.</returns>
    public static IBusRegistrationConfigurator UseExceptionHub(this IBusRegistrationConfigurator cfg)
    {
        cfg.TryAddScoped(typeof(ExceptionHandlingConsumeFilter<>));

        cfg.AddConfigureEndpointsCallback((context, name, ep) =>
        {
            ep.UseConsumeFilter(typeof(ExceptionHandlingConsumeFilter<>), context);
        });

        return cfg;
    }
}
