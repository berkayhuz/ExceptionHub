using System.Text.Json;

using ExceptionHub.Abstractions;
using ExceptionHub.Core.Utilities;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace ExceptionHub.Http;

/// <summary>
/// Implements a centralized exception handler for ASP.NET Core using the <see cref="IExceptionHandler"/> interface
/// introduced in .NET 8. This handler maps exceptions to structured responses using configured <see cref="IExceptionMapper"/> instances
/// and applies content negotiation to return standardized API error payloads.
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IExceptionMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
    /// </summary>
    /// <param name="mapper">The <see cref="IExceptionMapper"/> used to resolve exception mappings.</param>
    public GlobalExceptionHandler(IExceptionMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Attempts to handle an unhandled exception during the ASP.NET Core request pipeline.
    /// If the exception can be mapped and the response has not yet started,
    /// a structured <see cref="ApiProblemDetails"/> response is written to the client.
    /// </summary>
    /// <param name="ctx">The current HTTP context.</param>
    /// <param name="ex">The unhandled exception.</param>
    /// <param name="ct">A cancellation token for the request.</param>
    /// <returns>
    /// <c>true</c> if the exception was successfully handled and a response was written;
    /// <c>false</c> if the response has already started or the request did not accept <c>application/json</c>.
    /// </returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        if (ctx.Response.HasStarted)
            return false;

        var map = _mapper.Map(ex) ?? new(500, "unhandled_exception", ErrorType.Unexpected);

        if (!ctx.Request.Accepts("application/json", out _))
            return false;

        var pd = new ApiProblemDetails
        {
            Type = $"https://httpstatuses.io/{map.StatusCode}",
            Title = ReasonPhrases.GetReasonPhrase(map.StatusCode),
            Status = map.StatusCode,
            ErrorCode = map.ErrorCode,
            TraceId = ctx.TraceIdentifier,
            CorrelationId = ctx.Request.Headers["X-Correlation-Id"].FirstOrDefault()
        };

        ctx.Response.StatusCode = map.StatusCode;
        ctx.Response.ContentType = "application/problem+json";
        await JsonSerializer.SerializeAsync(ctx.Response.Body, pd, cancellationToken: ct);

        return true;
    }
}
