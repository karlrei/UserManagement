namespace UserManagementAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;

            try
            {
                await _next(context);
                var statusCode = context.Response.StatusCode;
                _logger.LogInformation("HTTP {Method} {Path} responded {StatusCode}", method, path, statusCode);
            }
            finally
            {
                // nothing to dispose that we replaced here
            }
        }
    }
}
