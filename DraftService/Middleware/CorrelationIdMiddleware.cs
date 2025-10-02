using Serilog;

namespace DraftService.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public const string HeaderName = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var cid)
            ? cid.ToString()
            : Guid.NewGuid().ToString();

        context.Response.Headers[HeaderName] = correlationId;

        using (Serilog.Context.LogContext.PushProperty("CorrelationId", correlationId))
        {
            Log.Information("➡️ {Method} {Path} started (CorrelationId={CorrelationId})",
                context.Request.Method, context.Request.Path, correlationId);

            await _next(context);

            Log.Information("⬅️ {Method} {Path} finished {StatusCode} (CorrelationId={CorrelationId})",
                context.Request.Method, context.Request.Path, context.Response.StatusCode, correlationId);
        }
    }
}
