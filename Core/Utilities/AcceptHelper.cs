using Microsoft.AspNetCore.Http;

namespace ExceptionHub.Core.Utilities;

/// <summary>
/// Provides helper methods for content negotiation by evaluating the <c>Accept</c> header of an HTTP request.
/// </summary>
internal static class AcceptHelper
{
    /// <summary>
    /// Determines whether the specified media type is accepted by the client, based on the <c>Accept</c> header.
    /// </summary>
    /// <param name="req">The current <see cref="HttpRequest"/>.</param>
    /// <param name="media">The media type to check for (e.g., <c>application/json</c>).</param>
    /// <param name="matched">
    /// When this method returns, contains the matched media type from the request header if found;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the request's <c>Accept</c> header includes the specified media type; otherwise, <c>false</c>.
    /// </returns>
    public static bool Accepts(this HttpRequest req, string media, out string? matched)
    {
        var accepts = req.Headers.Accept;
        if (accepts.Count == 0)
        {
            matched = null;
            return false;
        }

        matched = accepts.FirstOrDefault(h =>
            h is not null &&
            h.Contains(media, StringComparison.OrdinalIgnoreCase));

        return matched is not null;
    }
}
