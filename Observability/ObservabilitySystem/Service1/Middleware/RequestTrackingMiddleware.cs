using System.Diagnostics;

namespace Service1.Middleware;

public class RequestTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTrackingMiddleware> _logger;

    public RequestTrackingMiddleware(RequestDelegate next, ILogger<RequestTrackingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Start timing the request
        var stopwatch = Stopwatch.StartNew();
        var requestId = context.TraceIdentifier;

        // Log request start
        _logger.LogInformation("Request started: {Method} {Path} with RequestId: {RequestId}", 
            context.Request.Method, context.Request.Path, requestId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            
            // Log request completion
            _logger.LogInformation("Request completed: {Method} {Path} with RequestId: {RequestId} " +
                                 "in {ElapsedMs}ms with status {StatusCode}", 
                context.Request.Method, 
                context.Request.Path, 
                requestId,
                stopwatch.ElapsedMilliseconds, 
                context.Response.StatusCode);
        }
    }
}
