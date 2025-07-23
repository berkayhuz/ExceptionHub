namespace ExceptionHub.Abstractions;

/// <summary>
/// Defines a contract for mapping exceptions to structured <see cref="ExceptionMapping"/> metadata.
/// Implementations of this interface are responsible for transforming specific exception types
/// into consistent and meaningful mappings used for logging, response shaping, and observability.
/// </summary>
public interface IExceptionMapper
{
    /// <summary>
    /// Attempts to map the specified <paramref name="exception"/> to an <see cref="ExceptionMapping"/> instance.
    /// Return <c>null</c> to indicate that the exception is not recognized or should fall back to the default handler.
    /// </summary>
    /// <param name="exception">The exception to be mapped.</param>
    /// <returns>
    /// An <see cref="ExceptionMapping"/> describing how the exception should be logged and surfaced to clients,
    /// or <c>null</c> if the exception is not handled by this mapper.
    /// </returns>
    ExceptionMapping? Map(Exception exception);
}
