// #define SERILOG_RESPONSES       // Serilog logging/measurement. Otherwise - manual.

using Application;
using Application.Models;
using Common.JwtHelperService;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using WebAPI;
using WebAPI.Middleware;
using InfrastructureDependencyInjection = Infrastructure.InfrastructureDependencyInjection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"Minimal_API_{DateTime.Now:yyyyMMdd}.log")
    .CreateLogger();
Log.Information("----- STARTING -----");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger();
builder.Services.AddVersioning();

InfrastructureDependencyInjection.AddDataAccess(builder.Services.AddApplication(builder.Configuration), builder.Configuration)
                .AddServices(builder.Configuration)
                .AddJwtData(builder)
                .AddResponseCaching();
builder.Services.AddHealthChecks();

#if SERILOG_RESPONSES
builder.Services.AddSerilog();
#endif

var app = builder.Build();
// process exceptions in middleware. If some of them wasn't processed by middleware - will be processed in exception handler below
app.UseErrorHandlerMiddleware();

app.UseExceptionHandler(exceptionHandlerApp =>
{
	exceptionHandlerApp.Run(async httpContext =>
	{
		var pds = httpContext.RequestServices.GetService<IProblemDetailsService>();
		if (pds == null || !await pds.TryWriteAsync(new() { HttpContext = httpContext }))
		{
			// Fallback behavior
			await httpContext.Response.WriteAsync("Fallback: An error occurred.");
		}
	});
});

app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

app.MapGet("/exception", () =>
{
	throw new InvalidOperationException("Sample Exception");
});

app.UseMiddleware<ValidationExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
#if SERILOG_RESPONSES
    app.UseSerilogRequestLogging();
#endif
    app.UseCustomMiddleware();
}

// app.UseDefaultFiles(); // Enables serving default files like index.html
app.UseStaticFiles();  // Enables serving static files from wwwroot
app.UseHttpsRedirection();
app.UseResponseCaching();

Endpoints.MapAll(app);

if (!app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		var versionDescriptions = app.DescribeApiVersions();
//		var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
		foreach (var desc in versionDescriptions)
		{
			c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",$"Book API {desc.GroupName}");
		}

		c.DocumentTitle = "Book API";
	});
}
app.MapHealthChecks("/healthz");

if (!app.Environment.IsEnvironment("Testing"))
{
	app.MapPrometheusScrapingEndpoint();
	app.UseOpenTelemetryPrometheusScrapingEndpoint("metrics");

// default endpoint: /healthmetrics
	app.UseHealthChecksPrometheusExporter("/healthz");
}

// To test Environments: Development <=> Production in IIS Express in launchSettings.json
Console.WriteLine($"App name: {app.Services.GetRequiredService<IOptions<ApiSettings>>().Value.Name}, version: {app.Services.GetRequiredService<IOptions<ApiSettings>>().Value.Version}");

app.Run();

Log.Information("----- FINISHING -----");
Log.CloseAndFlush();

public partial class Program { }
