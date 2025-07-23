using ExceptionHub.Abstractions;

using Microsoft.Extensions.Logging;

namespace ExceptionHub.Configuration;

/// <summary>
/// Provides configuration options for ExceptionHub, allowing the registration of custom exception-to-mapping rules.
/// These mappings are used to override default behavior and provide more precise handling for specific exception types.
/// </summary>
public sealed class ExceptionHubOptions
{
    private readonly Dictionary<Type, ExceptionMapping> _overrides = new();

    /// <summary>
    /// Registers a custom mapping for the specified exception type.
    /// When the given exception type is encountered, the associated <see cref="ExceptionMapping"/> will be used
    /// instead of falling back to the default handler or other mappers.
    /// </summary>
    /// <typeparam name="TException">The type of exception to map.</typeparam>
    /// <param name="status">The HTTP status code to associate with the exception.</param>
    /// <param name="code">An application-specific error code for identifying the exception.</param>
    /// <param name="type">The logical <see cref="ErrorType"/> classification of the exception.</param>
    /// <param name="log">The <see cref="LogLevel"/> to use when logging this exception.</param>
    /// <returns>The same <see cref="ExceptionHubOptions"/> instance to support method chaining.</returns>
    public ExceptionHubOptions Map<TException>(
        int status, string code,
        ErrorType type = ErrorType.Unspecified,
        LogLevel log = LogLevel.Error)
        where TException : Exception
    {
        _overrides[typeof(TException)] = new(status, code, type, log);
        return this;
    }

    /// <summary>
    /// Attempts to retrieve a registered <see cref="ExceptionMapping"/> for the specified exception type.
    /// </summary>
    /// <param name="exType">The runtime type of the exception.</param>
    /// <param name="mapping">When this method returns, contains the mapping associated with the exception type, if found.</param>
    /// <returns><c>true</c> if a mapping exists for the given type; otherwise, <c>false</c>.</returns>
    internal bool TryGet(Type exType, out ExceptionMapping mapping)
        => _overrides.TryGetValue(exType, out mapping!);
}
