using System.Diagnostics;
using Application.Models;
using Infrastructure.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace WebAPI.Middleware;

public class CustomMiddleware
{
    private readonly RequestDelegate _next;
    private readonly PerformanceMetrics _metrics;

    public CustomMiddleware(RequestDelegate next, PerformanceMetrics metrics)
    {
        _next = next;
        _metrics = metrics;
    }

    public async Task InvokeAsync(HttpContext context)
    {
	    if ((context.GetEndpoint() != null && (context.GetEndpoint()!.DisplayName!.ToLower().Equals("/metrics")
	                                          || context.GetEndpoint()!.DisplayName!.ToLower().Equals("/swagger/v1/swagger.json")
	                                          || context.GetEndpoint()!.DisplayName!.ToLower().Equals("http: get /")))
	        || context.GetEndpoint() == null)
	    {
		    await _next(context);
		    return;
	    }
        var timer = Stopwatch.StartNew();
        var strCorrName = context.RequestServices.GetRequiredService<IOptions<ApiSettings>>().Value.CorrelationName;
        var correlationId = context.Request.Headers[strCorrName].FirstOrDefault();

        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Response.Headers[strCorrName] = correlationId;

        context.Items["CorrelationId"] = correlationId;

        // Call the next delegate/middleware in the pipeline.
        await _next(context);

        _metrics.RecordRequestTime(context.Request.Path, context.Request.Method, context.Response.StatusCode, timer.ElapsedMilliseconds, correlationId);

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