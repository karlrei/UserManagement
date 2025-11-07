using System.Text.Json;

namespace UserManagementAPI.Middleware
{
    public class SimpleTokenAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SimpleTokenAuthMiddleware> _logger;
        // For demo purposes: a hard-coded token. Replace with real validation in production.
        private const string DemoToken = "Bearer demo-token-123";

        public SimpleTokenAuthMiddleware(RequestDelegate next, ILogger<SimpleTokenAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Allow anonymous access to Swagger and health checks
            var path = context.Request.Path;
            if (path.StartsWithSegments("/swagger") || path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Authorization", out var auth))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unauthorized" }));
                return;
            }

            if (!string.Equals(auth.ToString(), DemoToken, StringComparison.Ordinal))
            {
                _logger.LogWarning("Invalid token for request {Method} {Path}", context.Request.Method, context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Unauthorized" }));
                return;
            }

            await _next(context);
        }
    }
}
