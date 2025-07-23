using System.ComponentModel.DataAnnotations;

using ExceptionHub.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExceptionHub.Core.Mapping;

/// <summary>
/// Provides a set of conservative default mappings for common exception types.
/// These mappings define standard HTTP status codes, error codes, error types, and log levels for
/// well-known exceptions. This implementation is intended to be overridden or extended via dependency injection
/// to support domain-specific exception handling.
/// </summary>
internal sealed class DefaultExceptionMapper : IExceptionMapper
{
    /// <summary>
    /// Maps the provided exception to a default <see cref="ExceptionMapping"/>, if recognized.
    /// Returns <c>null</c> for unrecognized exceptions, allowing fallback to other mappers or default handling.
    /// </summary>
    /// <param name="ex">The exception instance to map.</param>
    /// <returns>
    /// A predefined <see cref="ExceptionMapping"/> if the exception type is known;
    /// otherwise, <c>null</c>.
    /// </returns>
    public ExceptionMapping? Map(Exception ex) => ex switch
    {
        ValidationException or ArgumentException
            => new(StatusCodes.Status400BadRequest, "validation_error",
                   ErrorType.Validation, LogLevel.Warning),

        KeyNotFoundException
            => new(StatusCodes.Status404NotFound, "not_found", ErrorType.NotFound, LogLevel.Information),

        UnauthorizedAccessException
            => new(StatusCodes.Status401Unauthorized, "unauthorized", ErrorType.Unauthorized, LogLevel.Warning),

        OperationCanceledException
            => new(499, "client_abort", ErrorType.Unexpected, LogLevel.Debug),

        _ => null
    };
}
