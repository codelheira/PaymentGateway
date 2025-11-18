using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace PaymentGateway.Api.Middleware;

public class TracingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ActivitySource _activitySource;
    private readonly ILogger<TracingMiddleware> _logger;

    public TracingMiddleware(RequestDelegate next, ActivitySource activitySource, ILogger<TracingMiddleware> logger)
    {
        _next = next;
        _activitySource = activitySource;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        using var activity = _activitySource.StartActivity($"{context.Request.Method} {context.Request.Path}", ActivityKind.Server);
        if (activity is not null)
        {
            activity.SetTag("http.method", context.Request.Method);
            activity.SetTag("http.target", context.Request.Path);
            activity.SetTag("http.scheme", context.Request.Scheme);
            activity.SetTag("correlation_id", correlationId);
        }

        context.Response.Headers["X-Correlation-ID"] = correlationId;

        // Use IEnumerable<KeyValuePair<string, object?>> to ensure console logger serializa corretamente
        var scopeItems = new[]
        {
            new KeyValuePair<string, object?>("TraceId", activity?.TraceId.ToString()),
            new KeyValuePair<string, object?>("SpanId", activity?.SpanId.ToString()),
            new KeyValuePair<string, object?>("CorrelationId", correlationId)
        };

        using (_logger.BeginScope(scopeItems))
        {
            _logger.LogInformation("Request start {Method} {Path}", context.Request.Method, context.Request.Path);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception during request");
                throw;
            }
            finally
            {
                _logger.LogInformation("Request finished {Method} {Path} StatusCode={StatusCode}", context.Request.Method, context.Request.Path, context.Response?.StatusCode);
            }
        }
    }
}