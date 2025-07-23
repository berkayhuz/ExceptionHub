using ExceptionHub.Abstractions;

using MediatR;

using Microsoft.AspNetCore.Http;

namespace ExceptionHub.Mediator;

/// <summary>
/// A MediatR pipeline behavior that intercepts unhandled exceptions thrown during request processing
/// and delegates them to <see cref="IExceptionMapper"/> for structured classification.
/// This ensures consistent exception handling across application-layer requests, including commands and queries.
/// </summary>
/// <typeparam name="TRequest">The type of the MediatR request.</typeparam>
/// <typeparam name="TResponse">The type of the response returned by the request handler.</typeparam>
public sealed class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IExceptionMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="mapper">The exception mapper used to classify exceptions.</param>
    public ExceptionHandlingBehavior(IExceptionMapper mapper)
    {
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var mapping = _mapper.Map(ex)
                       ?? new ExceptionMapping(StatusCodes.Status500InternalServerError, "unhandled_exception", ErrorType.Unexpected);

            throw;
        }
    }
}
