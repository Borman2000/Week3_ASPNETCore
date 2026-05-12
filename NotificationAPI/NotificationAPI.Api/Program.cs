using Common.JwtHelperService;
using Common.OpenTelemetryService;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationAPI.Api;
using NotificationAPI.Application;
using NotificationAPI.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File($"NotificationAPI_{DateTime.Now.ToString("yyyyMMdd")}.log")
	.CreateLogger();
Log.Information("----- STARTING -----");

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddGeneralServices()
	.AddDataAccess(builder.Configuration)
	.AddApplication(builder.Configuration)
	.AddJwtData(builder)
	.AddHealthChecks();

builder.Services.AddOpenTelemetryTracing(builder.Configuration);
builder.Services.AddOpenTelemetryMetrics(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

Endpoints.Map(app);

if (!app.Environment.IsProduction())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notification API v1");
		c.DocumentTitle = "Notification API";
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

app.Run();