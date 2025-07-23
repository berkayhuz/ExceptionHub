using ExceptionHub.Abstractions;

namespace ExceptionHub.Core.Mapping;

/// <summary>
/// An <see cref="IExceptionMapper"/> implementation that delegates exception mapping
/// to a sequence of inner mappers and returns the first successful match.
/// This enables composition of multiple mapping strategies or modules.
/// </summary>
internal sealed class CompositeExceptionMapper : IExceptionMapper
{
    private readonly IReadOnlyList<IExceptionMapper> _mappers;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeExceptionMapper"/> class
    /// with the specified collection of delegate mappers.
    /// </summary>
    /// <param name="mappers">A collection of <see cref="IExceptionMapper"/> instances to evaluate in order.</param>
    public CompositeExceptionMapper(IEnumerable<IExceptionMapper> mappers)
        => _mappers = mappers.ToList();

    /// <summary>
    /// Iterates through the registered exception mappers and returns the first non-null mapping result.
    /// </summary>
    /// <param name="ex">The exception to be mapped.</param>
    /// <returns>
    /// The first non-null <see cref="ExceptionMapping"/> returned by the inner mappers,
    /// or <c>null</c> if none of them can handle the exception.
    /// </returns>
    public ExceptionMapping? Map(Exception ex)
    {
        foreach (var m in _mappers)
            if (m.Map(ex) is { } mapped)
                return mapped;

        return null;
    }
}
