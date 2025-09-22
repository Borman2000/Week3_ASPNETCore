using System.Diagnostics;
using Microsoft.Extensions.Options;
using Serilog;

namespace WebAPI;

public class CustomMiddleware
{
    private readonly RequestDelegate _next;

    public CustomMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var timer = Stopwatch.StartNew();
        var strCorrName = context.RequestServices.GetRequiredService<IOptions<CustomOptions>>().Value.CorrelationName;
        var correlationId = context.Request.Headers[strCorrName].FirstOrDefault();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Response.Headers[strCorrName] = correlationId;

        context.Items["CorrelationId"] = correlationId;

        // Call the next delegate/middleware in the pipeline.
        await _next(context);

#if !SERILOG_RESPONSES
        Log.Information($"Request {correlationId}: ({context.Request.Method} {context.Request.Path}) processed in {timer.ElapsedMilliseconds} ms with status response {context.Response.StatusCode}");
#endif
    }
}

public static class CustomMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomMiddleware>();
    }
}