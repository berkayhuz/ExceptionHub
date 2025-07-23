using Microsoft.AspNetCore.Mvc;

namespace ExceptionHub.Http;

/// <summary>
/// Represents a structured API error response that extends <see cref="ProblemDetails"/>
/// with additional fields commonly used for request tracing and application-specific error reporting.
/// </summary>
public sealed class ApiProblemDetails : ProblemDetails
{
    /// <summary>
    /// Gets or sets the unique trace identifier associated with the request.
    /// Typically derived from <c>HttpContext.TraceIdentifier</c>.
    /// </summary>
    public string? TraceId
    {
        get; init;
    }

    /// <summary>
    /// Gets or sets the correlation ID used to associate this request with a larger transaction or operation context.
    /// Often extracted from an <c>X-Correlation-ID</c> header.
    /// </summary>
    public string? CorrelationId
    {
        get; init;
    }

    /// <summary>
    /// Gets or sets the application-defined error code.
    /// Useful for client-side error categorization and localization.
    /// </summary>
    public string? ErrorCode
    {
        get; init;
    }
}
