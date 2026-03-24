using AuthAPI.Api;
using AuthAPI.Infrastructure;
using AuthAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File($"Users_API_{DateTime.Now.ToString("yyyyMMdd")}.log")
	.CreateLogger();
Log.Information("----- STARTING -----");

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("JwtConfig.json", optional: true);

builder.Services
	.AddGeneralServices()
	.AddDataAccess(builder.Configuration)
	.AddJwtData(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication(); // Must come before UseAuthorization
app.UseAuthorization();

Endpoints.Map(app);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAPI.Api V1"); });
}

// Create and seed database
using (var scope = app.Services.CreateScope())
{
	await UsersSeedService.SeedAsync(scope.ServiceProvider);
}

app.Run();