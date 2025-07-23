using ExceptionHub.Abstractions;
using ExceptionHub.Configuration;

namespace ExceptionHub.Core.Mapping;

/// <summary>
/// An <see cref="IExceptionMapper"/> implementation that resolves exception mappings
/// based on user-defined configuration provided via <see cref="ExceptionHubOptions"/>.
/// This allows consumers to register custom exception mappings during application startup.
/// </summary>
internal sealed class OptionsExceptionMapper : IExceptionMapper
{
    private readonly ExceptionHubOptions _opts;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptionsExceptionMapper"/> class
    /// using the specified <see cref="ExceptionHubOptions"/> source.
    /// </summary>
    /// <param name="opts">The options instance containing custom exception mappings.</param>
    public OptionsExceptionMapper(ExceptionHubOptions opts) => _opts = opts;

    /// <summary>
    /// Attempts to retrieve a user-defined mapping for the provided exception instance.
    /// </summary>
    /// <param name="ex">The exception to map.</param>
    /// <returns>
    /// A configured <see cref="ExceptionMapping"/> if one exists for the exception's type;
    /// otherwise, <c>null</c> to allow fallback to other handlers.
    /// </returns>
    public ExceptionMapping? Map(Exception ex)
        => _opts.TryGet(ex.GetType(), out var m) ? m : null;
}
