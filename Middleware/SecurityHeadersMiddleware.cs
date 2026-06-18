namespace CV.Middleware;

/// <summary>
/// Adds defensive HTTP response headers to every request:
/// clickjacking protection, MIME-sniffing protection, a strict
/// Content-Security-Policy and a tight referrer / permissions policy.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    // Locked-down CSP. The site only loads its own scripts/styles plus Google
    // Fonts; everything else (frames, objects, foreign origins) is denied.
    private const string ContentSecurityPolicy =
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com; " +
        "img-src 'self' data:; " +
        "connect-src 'self'; " +
        "form-action 'self'; " +
        "base-uri 'self'; " +
        "frame-ancestors 'none'; " +
        "object-src 'none'; " +
        "upgrade-insecure-requests";

    public SecurityHeadersMiddleware(RequestDelegate next) => _next = next;

    public Task InvokeAsync(HttpContext context)
    {
        var headers = context.Response.Headers;

        headers["X-Content-Type-Options"] = "nosniff";
        headers["X-Frame-Options"] = "DENY";
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=(), payment=()";
        headers["Content-Security-Policy"] = ContentSecurityPolicy;
        headers["Cross-Origin-Opener-Policy"] = "same-origin";

        // Don't advertise the server technology.
        headers.Remove("X-Powered-By");

        return _next(context);
    }
}
