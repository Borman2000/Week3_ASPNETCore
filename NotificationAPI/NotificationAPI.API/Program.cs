using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NotificationAPI.API;
using NotificationAPI.Application;
using NotificationAPI.Infrastructure;
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
	.AddApplication(builder.Configuration);
//	.AddJwtData(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
//app.UseAuthentication(); // Must come before UseAuthorization
//app.UseAuthorization();

Endpoints.Map(app);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotificationAPI.Api V1"); });
}

app.Run();