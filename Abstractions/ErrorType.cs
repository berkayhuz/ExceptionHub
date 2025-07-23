namespace ExceptionHub.Abstractions;

/// <summary>
/// Represents the type or category of an error that occurred within the application.
/// This classification is used by the exception mapping infrastructure to provide structured metadata
/// for logging, problem details, and response shaping.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// The error type is not specified. This is the default value and should be avoided
    /// in production scenarios where meaningful classification is expected.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Indicates a validation error, such as input that failed to meet defined rules or constraints.
    /// Typically mapped to HTTP 400 (Bad Request).
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Indicates that the requested resource could not be found.
    /// Typically mapped to HTTP 404 (Not Found).
    /// </summary>
    NotFound = 2,

    /// <summary>
    /// Indicates a conflict with the current state of the resource.
    /// For example, trying to create a duplicate entry. Typically mapped to HTTP 409 (Conflict).
    /// </summary>
    Conflict = 3,

    /// <summary>
    /// Indicates that the user is not authenticated or has failed to provide valid credentials.
    /// Typically mapped to HTTP 401 (Unauthorized).
    /// </summary>
    Unauthorized = 4,

    /// <summary>
    /// Indicates that the user is authenticated but does not have permission to perform the action.
    /// Typically mapped to HTTP 403 (Forbidden).
    /// </summary>
    Forbidden = 5,

    /// <summary>
    /// Indicates an unexpected or unhandled error. This type is used for unknown failures or fallback cases.
    /// Typically mapped to HTTP 500 (Internal Server Error).
    /// </summary>
    Unexpected = 10
}
