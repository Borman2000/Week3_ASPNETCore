using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StackExchange.Redis;

namespace Common.OpenTelemetryService;

public static class OpenTelemetryExtensions
{
    public static void AddOpenTelemetryTracing(this IServiceCollection services, IConfiguration configuration)
    {
		services.Configure<OpenTelemetryParameters>(configuration.GetSection("OpenTelemetry"));
        var otlpParams = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryParameters>();

		ActivitySourceProvider.SetSource(otlpParams!.ActivitySourceName);

        services.AddOpenTelemetry().WithTracing(options =>
        {
            options
	            .AddSource("Yarp.ReverseProxy")
	            .AddSource(otlpParams.ActivitySourceName)
                .AddSource("MassTransit")
                .ConfigureResource(resource =>
                {
                    resource.AddService(otlpParams.ServiceName,
                        serviceVersion: otlpParams.ServiceVersion);
                });

            options.AddAspNetCoreInstrumentation(o =>
            {
                // to trace only api requests
//                o.Filter = (context) => !string.IsNullOrEmpty(context.Request.Path.Value) && context.Request.Path.Value.Contains("Api", StringComparison.InvariantCulture);
                o.Filter = request =>
                {
	                var path = request.Request.Path.Value;
	                return !string.IsNullOrEmpty(path) &&
	                       !path.StartsWith("/healthz") &&
	                       !path.StartsWith("/metrics") &&
	                       !path.StartsWith("/swagger") &&
	                       !path.StartsWith("/favicon.ico");
                };

                // example: only collect telemetry about HTTP GET requests
                // return httpContext.Request.Method.Equals("GET");

                // enrich activity with http request and response
                o.EnrichWithHttpRequest = (activity, httpRequest) => { activity.SetTag("requestProtocol", httpRequest.Protocol); };
                o.EnrichWithHttpResponse = (activity, httpResponse) => { activity.SetTag("responseLength", httpResponse.ContentLength); };

                // automatically sets Activity Status to Error if an unhandled exception is thrown
                o.RecordException = true;
                o.EnrichWithException = (activity, exception) =>
                {
                    activity.SetTag("exceptionType", exception.GetType().ToString());
                    activity.SetTag("stackTrace", exception.StackTrace);
                };
            });

// Note that the AddHttpClientInstrumentation() call is required along with the AddSource("Yarp.ReverseProxy") call to make the request spans emit.
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/yarp/distributed-tracing
            options.AddHttpClientInstrumentation();

            options.AddEntityFrameworkCoreInstrumentation(opt =>
            {
//                opt.SetDbStatementForText = true;
//                opt.SetDbStatementForStoredProcedure = true;
                opt.EnrichWithIDbCommand = (activity, command) =>
                {
                    // var stateDisplayName = $"{command.CommandType} main";
                    // activity.DisplayName = stateDisplayName;
                    // activity.SetTag("db.name", stateDisplayName);
                };
            });

            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]!);
            options
	            .AddRedisInstrumentation(connectionMultiplexer)
	            .AddMySqlDataInstrumentation(opt => opt.RecordException = true);


            if(otlpParams.IsUseConsole)
				options.AddConsoleExporter();

            options.AddOtlpExporter(opt =>
            {
	            opt.Endpoint = new Uri(otlpParams.Endpoint);
            });
        });
    }

    public static MeterProviderBuilder WithInstrumentation<T>(this MeterProviderBuilder builder) where T : class
    {
	    return builder.AddInstrumentation<T>();
    }

    public static MeterProviderBuilder AddOpenTelemetryMetrics(this IServiceCollection services, IConfiguration configuration)
    {
// TODO: Is it suitable approach? I need ability to add instrumentation if needed
	    MeterProviderBuilder? outerBuilder = null;

	    services.Configure<OpenTelemetryParameters>(configuration.GetSection("OpenTelemetry"));
        var otlpParams = configuration.GetSection("OpenTelemetry").Get<OpenTelemetryParameters>();
        
        services.AddOpenTelemetry().WithMetrics(builder =>
        {
	        outerBuilder = builder;
            builder
	            .AddMeter(otlpParams!.ServiceName)
	            // Metrics provider from OpenTelemetry
	            .AddAspNetCoreInstrumentation()
	            // Metrics provides by ASP.NET Core in .NET 8
	            .AddMeter("Microsoft.AspNetCore.Hosting")
	            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
	            // Metrics provided by System.Net libraries
	            .AddMeter("System.Net.Http")
	            .AddMeter("System.Net.NameResolution")
				.AddPrometheusExporter();

            builder.ConfigureResource(resource =>
            {
                resource.AddService(serviceName: otlpParams.ServiceName,
                    serviceVersion: otlpParams.ServiceVersion);
            });

            builder.AddOtlpExporter(opt =>
            {
	            opt.Endpoint = new Uri(otlpParams.Endpoint);
            });
        });

        return outerBuilder!;
    }
}