// #define SERILOG_RESPONSES       // Serilog logging/measurement. Otherwise - manual.

using Application;
using Application.Models;
using AuthAPI.Infrastructure;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using WebAPI;
using WebAPI.Middleware;
using InfrastructureDependencyInjection = Infrastructure.InfrastructureDependencyInjection;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File($"Minimal_API_{DateTime.Now.ToString("yyyyMMdd")}.log")
    .CreateLogger();
Log.Information("----- STARTING -----");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("JwtConfig.json", optional: false);

builder.Services.AddSwagger();
InfrastructureDependencyInjection.AddDataAccess(builder.Services.AddApplication(builder.Configuration), builder.Configuration)
                .AddServices(builder.Configuration)
                .AddJwtData(builder.Configuration)
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
if (app.Environment.IsDevelopment())
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

Endpoints.MapCQRS(app);
//Endpoints.Map(app);

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI V1"); });
app.MapHealthChecks("/health");

app.MapPrometheusScrapingEndpoint();
app.UseOpenTelemetryPrometheusScrapingEndpoint("metrics");

// default endpoint: /healthmetrics
app.UseHealthChecksPrometheusExporter("/healthmetrics");

// To test Environments: Development <=> Production in IIS Express in launchSettings.json
Console.WriteLine($"App name: {app.Services.GetRequiredService<IOptions<ApiSettings>>().Value.Name}, version: {app.Services.GetRequiredService<IOptions<ApiSettings>>().Value.Version}");

app.Run();

Log.Information("----- FINISHING -----");
Log.CloseAndFlush();

public partial class Program { }
