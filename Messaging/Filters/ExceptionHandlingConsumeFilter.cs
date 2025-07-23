using ExceptionHub.Abstractions;

using MassTransit;

using Microsoft.AspNetCore.Http;

namespace ExceptionHub.Messaging.Filters;

/// <summary>
/// A MassTransit consume filter that intercepts unhandled exceptions thrown by consumers
/// and maps them using <see cref="IExceptionMapper"/>. Mapped exceptions are rethrown to allow MassTransit
/// to handle them through its built-in fault and dead-letter mechanisms.
/// </summary>
/// <typeparam name="T">The message contract type being consumed.</typeparam>
public sealed class ExceptionHandlingConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IExceptionMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingConsumeFilter{T}"/> class.
    /// </summary>
    /// <param name="mapper">The exception mapper responsible for classifying exceptions.</param>
    public ExceptionHandlingConsumeFilter(IExceptionMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        try
        {
            await next.Send(context);
        }
        catch (Exception ex)
        {
            var mapping = _mapper.Map(ex)
                       ?? new ExceptionMapping(StatusCodes.Status500InternalServerError, "unhandled_exception", ErrorType.Unexpected);

            throw; // Let MassTransit handle via fault or DLQ
        }
    }

    /// <inheritdoc />
    public void Probe(ProbeContext context) => context.CreateScope("ExceptionHubConsumeFilter");
}
