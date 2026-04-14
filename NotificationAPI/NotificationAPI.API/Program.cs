using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
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

builder.Configuration.AddJsonFile("JwtConfig.json", optional: false);

builder.Services
	.AddGeneralServices()
	.AddDataAccess(builder.Configuration)
	.AddApplication(builder.Configuration)
	.AddHealthChecks();
//	.AddJwtData(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
//app.UseAuthentication(); // Must come before UseAuthorization
//app.UseAuthorization();

Endpoints.Map(app);

if (app.Environment.IsDevelopment())
{
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI(c =>
		{
			c.SwaggerEndpoint("/swagger/v2/swagger.json", "Notification API v1");
			c.DocumentTitle = "Notification API";
		});
	}
}
app.MapHealthChecks("/healthz");

app.Run();