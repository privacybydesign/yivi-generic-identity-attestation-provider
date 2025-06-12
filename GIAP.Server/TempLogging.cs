using System.Text;

namespace GIAP.Server;

// todo temp fixing
public class HeaderSizeLoggingMiddleware
{
    private readonly ILogger<HeaderSizeLoggingMiddleware> _logger;
    private readonly RequestDelegate _next;

    public HeaderSizeLoggingMiddleware(RequestDelegate next, ILogger<HeaderSizeLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture the original response stream
        var originalResponseStream = context.Response.Body;

        using var responseStream = new MemoryStream();
        context.Response.Body = responseStream;

        await _next(context);

        // Calculate header sizes after the response is generated
        var totalHeaderSize = 0;
        var largestHeader = "";
        var largestHeaderSize = 0;

        foreach (var header in context.Response.Headers)
        {
            var headerString = $"{header.Key}: {string.Join(", ", header.Value)}";
            var headerSize = Encoding.UTF8.GetByteCount(headerString);
            totalHeaderSize += headerSize;

            if (headerSize > largestHeaderSize)
            {
                largestHeaderSize = headerSize;
                largestHeader = header.Key;
            }

            // Log individual large headers (over 1KB)
            if (headerSize > 1024)
            {
                _logger.LogWarning("Large header detected: {HeaderName} = {HeaderSize} bytes",
                    header.Key, headerSize);
            }
        }

        // Log total header size for OIDC callback requests specifically
        if (context.Request.Path.StartsWithSegments("/api/callback-signin-msentraid") ||
            context.Request.Path.StartsWithSegments("/api/callback-signin-caesar"))
        {
            _logger.LogWarning("OIDC Callback - Total response header size: {TotalSize} bytes, " +
                               "Largest header: {LargestHeader} ({LargestSize} bytes), " +
                               "Status: {StatusCode}",
                totalHeaderSize, largestHeader, largestHeaderSize, context.Response.StatusCode);

            // Log all Set-Cookie headers specifically
            if (context.Response.Headers.ContainsKey("Set-Cookie"))
            {
                var cookies = context.Response.Headers["Set-Cookie"];
                for (int i = 0; i < cookies.Count; i++)
                {
                    var cookieSize = Encoding.UTF8.GetByteCount(cookies[i]);
                    _logger.LogWarning("Set-Cookie[{Index}]: {CookieSize} bytes", i, cookieSize);
                }
            }
        }

        // Copy the response back to the original stream
        responseStream.Seek(0, SeekOrigin.Begin);
        await responseStream.CopyToAsync(originalResponseStream);
    }
}