using AuthAPI.Api;
using AuthAPI.Infrastructure;
using AuthAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File($"AuthAPI_{DateTime.Now.ToString("yyyyMMdd")}.log")
	.CreateLogger();
Log.Information("----- STARTING -----");

var builder = WebApplication.CreateBuilder(args);

builder.Services
	.AddGeneralServices()
	.AddDataAccess(builder.Configuration)
	.AddJwtData(builder)
	.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

Endpoints.Map(app);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth API v1");
		c.DocumentTitle = "Auth API";
	});
}

app.MapHealthChecks("/healthz");

// Create and seed database
using (var scope = app.Services.CreateScope())
{
	await UsersSeedService.SeedAsync(scope.ServiceProvider);
}

app.Run();