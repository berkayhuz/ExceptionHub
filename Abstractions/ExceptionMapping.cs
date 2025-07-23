using Microsoft.Extensions.Logging;

namespace ExceptionHub.Abstractions;

/// <summary>
/// Represents the structured metadata associated with an exception.
/// This mapping is used to transform exceptions into meaningful HTTP responses,
/// log messages, and API error payloads.
/// </summary>
public sealed record ExceptionMapping(
    /// <summary>
    /// The HTTP status code to associate with the exception.
    /// For example, 400 for validation errors, 404 for missing resources, or 500 for internal server errors.
    /// </summary>
    int StatusCode,

    /// <summary>
    /// An optional application-specific error code to help identify and categorize the error in a domain-specific way.
    /// This value is commonly included in the response payload for front-end or client-side error handling.
    /// </summary>
    string? ErrorCode = null,

    /// <summary>
    /// A classification of the error type, such as Validation, NotFound, or Unexpected.
    /// Used for consistent error semantics and observability across services.
    /// </summary>
    ErrorType ErrorType = ErrorType.Unspecified,

    /// <summary>
    /// The severity level to be used when logging the exception.
    /// For example, use <see cref="LogLevel.Warning"/> for recoverable issues or <see cref="LogLevel.Error"/> for critical failures.
    /// </summary>
    LogLevel LogLevel = LogLevel.Error);
